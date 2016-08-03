using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Akka.Actor;
using Akka.Event;
using Couchbase.Lite;
using Even.Persistence.CouchbaseLite.Messages;

namespace Even.Persistence.CouchbaseLite
{
    internal class MonotonicallyIncreasingSequence : ReceiveActor, IWithUnboundedStash
    {
        public static readonly string EvenSequenceNumberPropertyName = "NextSequenceNumber";

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        
        public MonotonicallyIncreasingSequence(Database database)
        {
            Db = database;
            Stopped();
        }

        public static Props CreateProps(Database database)
            => Props.Create(() => new MonotonicallyIncreasingSequence(database));

        public IStash Stash { get; set; }
        public Database Db { get; }
        public bool IsFaulted { get; private set; }
        public long Next { get; private set; }

        protected override void PreStart()
        {
            Self.Tell(StartSequence.Instance);
        }

        private void Stopped()
        {
            Receive<StartSequence>(cmd =>
            {
                if (Db == null)
                {
                    BecomeFaulted();
                }
                else
                {
                    BecomeStarting();
                    Self.Forward(cmd);
                }
            });

            Receive<SequenceStarted>(evt =>
            {
                BecomeStarted();
            });

            Receive<object>(_ => Stash.Stash());
        }

        private void Faulted()
        {
            Receive<ReserveSequenceNumbers>(_ =>
            {
                Sender.Tell(new SequenceNumberReservationRejected("The sequence is currently faulted and cannot generate sequence numbers."));
            });
        }

        private void Starting()
        {
            Receive<StartSequence>(_ =>
            {
                _log.Debug("Attempting to retrieve document with DocumentId: {0}", CouchbaseLiteStore.GlobalsDocumentId);
                var document = Db.GetDocument(CouchbaseLiteStore.GlobalsDocumentId);
                object value = -1;
                if (document.Properties != null && document.Properties.TryGetValue(EvenSequenceNumberPropertyName, out value) == true)
                {                    
                    //TODO: We should add some defensive code around the wrong values being here
                    Next = (long)value;

                    _log.Debug("Even sequence number document exists using Next: {0}", Next);
                }
                else
                {
                    var rev = document.PutProperties(new Dictionary<string, object>
                    {
                        {EvenSequenceNumberPropertyName, (long)1 }
                    });
                    
                    Next = (long)rev.GetProperty(EvenSequenceNumberPropertyName);

                    _log.Debug("Even sequence number document created using Next: {0}", Next);
                }

                _log.Debug("Starting with Next: {0}", Next);
                BecomeStarted();

                _log.Debug("Publishing SequenceStarted notification to EventStream");
                var sequenceStarted = new SequenceStarted(Next);
                Context.System.EventStream.Publish(sequenceStarted);
            });

            Receive<object>(_ => Stash.Stash());
        }

        private void Started()
        {
            Receive<IncrementSequence>(cmd =>
            {
                Next += cmd.Increment;
            });

            Receive<ReserveSequenceNumbers>(cmd =>
            {
                if (cmd.NumberToReserve == 0)
                {
                    Sender.Tell(
                        new SequenceNumberReservationRejected(
                            "Cannot reserve 0 sequences. The number to reserve must be greater than 0."));
                }
                else
                {
                    var start = Next + 1;                    
                    Next += cmd.NumberToReserve;
                    var end = Next;
                    Sender.Tell(new SequenceNumbersReserved(start,end));
                }
            });
        }

        private void BecomeFaulted()
        {
            IsFaulted = true;
            Become(Faulted);
        }

        private void BecomeStarting()
        {
            IsFaulted = false;
            Become(Starting);
        }

        private void BecomeStarted()
        {
            IsFaulted = false;
            Become(Started);
            Stash.UnstashAll();
        }        
    }
}