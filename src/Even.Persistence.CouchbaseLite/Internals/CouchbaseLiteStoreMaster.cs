using System;
using System.Collections.Generic;
using Akka.Actor;
using Couchbase.Lite;
using Even.Persistence.CouchbaseLite.Messages;

namespace Even.Persistence.CouchbaseLite.Internals
{
    internal class CouchbaseLiteStoreMaster : ReceiveActor, IWithUnboundedStash
    {
        private IActorRef _listener;
        public CouchbaseLiteStoreMaster(Database db, ISequenceGenerator sequence)
        {
            Db = db;
            Sequence = sequence;
            Stopped();
        }

        public static Props CreateProps(Database db, ISequenceGenerator sequence) =>
            Props.Create(() => new CouchbaseLiteStoreMaster(db, sequence));

        public IStash Stash { get; set; }
        public Database Db { get; }
        public ISequenceGenerator Sequence { get; }

        private void Stopped()
        {
            Receive<StartStore>(cmd =>
            {
                _listener = Sender;
                if (Db == null || Sequence == null)
                {
                    Become(Faulted);

                    var errors = new List<Exception>();
                    if (Db == null)
                    {
                        errors.Add(new InvalidOperationException("Cannot startup the store master when the database is null."));
                        
                    }

                    if (Sequence == null)
                    {
                        errors.Add(new InvalidOperationException("Cannot startup the store master when the sequence is null."));
                    }

                    Notify(OperationCompleted.WithErrors(typeof(StartStore), errors));
                }

                Become(Starting);
                Sender.Forward(cmd);                
            });

            Receive<object>(_ => Stash.Stash());
        }

        private void Starting()
        {
            Receive<StartStore>(cmd =>
            {
                Become(Started);
                Sender.Tell(OperationCompleted.Successfully(cmd.GetType()));                
            });
        }

        private void Started()
        {
            Receive<WriteEvents>(cmd =>
            {

                foreach (var @event in cmd.Events)
                {
                    //@event.
                }
            });
        }

        private void Faulted()
        {

        }
        
        private void Notify<TMessage>(TMessage message)
        {
            _listener?.Tell(message);
        }

        private void EnsureViews()
        {
            //Db.GetView(CouchbaseLiteStore.EventsViewName).CreateQuery().
        }

        private void GenerateViews()
        {
            var view = Db.GetView(CouchbaseLiteStore.EventsViewName);
            view.SetMap((doc, emit) =>
            {
                object documentType;
                if (doc.TryGetValue("documentType", out documentType))
                {
                    emit(documentType, null);
                }
            }, "1");
        }
    }
}