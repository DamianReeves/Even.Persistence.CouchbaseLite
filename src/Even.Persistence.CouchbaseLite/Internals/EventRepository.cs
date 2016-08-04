using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Lite;

namespace Even.Persistence.CouchbaseLite.Internals
{
    public class EventRepository : IEventRepository
    {
        internal const int PayloadSizeCutoffForBody = 500000;
        internal const int MetadataSizeCutoffForBody = 500000;

        public EventRepository(Database db, ISequenceGenerator sequence)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            Db = db;
            Sequence = sequence;
            InitializeViews();
        }

        public Database Db { get; }
        public ISequenceGenerator Sequence { get; }
        internal View EventsView { get; private set; }
        internal View EventsByStreamName { get; private set; }
        internal View EventsByEventId { get; private set; }

        public async Task<long> WriteEvent(IUnpersistedRawEvent @event)
        {            
            var documentId = $"event:{@event.EventID}";
            var document = Db.GetExistingDocument(documentId);
            if (document != null)
            {
                throw new DuplicatedEntryException();
            }

            if (@event.GlobalSequence == 0)
            {
                @event.GlobalSequence = await Sequence.Next(TimeSpan.FromSeconds(2));
            }

            document = Db.GetDocument(documentId);
            var rev = document.CreateRevision();
            var properties = new ExpandoObject();
            dynamic props = properties;
            props.documentType = "event";
            props.eventType = @event.EventType;
            props.eventId = @event.EventID;
            props.globalSequence = @event.GlobalSequence;
            props.payloadFormat = @event.PayloadFormat;
            props.utcTimeStamp = @event.UtcTimestamp;

            var eventWithStream = @event as IUnpersistedRawStreamEvent;
            if (eventWithStream?.Stream != null)
            {
                var streamId = await WriteStream(eventWithStream.Stream);
                props.streamId = streamId;
            }

            var payloadSize = @event.Payload?.Length ?? 0;
            props.payloadSize = payloadSize;

            if (payloadSize <= PayloadSizeCutoffForBody)// If payload size is bigger than ~0.5Mb save payload as an attachement
            {
                props.payloadInBody = true;
                props.payload = @event.Payload;
            }
            else
            {
                props.payloadInBody = false;
                rev.SetAttachment("payload", $"payloadFormat({@event.PayloadFormat})", @event.Payload);
            }

            var metadataSize = @event.Metadata?.Length ?? 0;
            if (metadataSize <= MetadataSizeCutoffForBody)
            {
                props.metadataInBody = true;
                props.metadata = @event.Metadata;
            }
            else
            {
                props.metadataInBody = false;
                rev.SetAttachment("metadata", $"payloadFormat({@event.PayloadFormat})", @event.Metadata);
            }

            rev.SetUserProperties(props);
            rev.Save();

            return @event.GlobalSequence;
        }

        private Task<string> WriteStream(Stream stream)
        {
            var streamId = $"event-stream:{stream.Name}";
            var doc = Db.GetDocument(streamId);
            var properties = new ExpandoObject();
            dynamic props = properties;
            bool shouldSave = false;
            if (doc.CurrentRevision == null)
            {
                shouldSave = true;
                props.documentType = "event-stream";
                props.name = stream.Name;
                props.originalStreamName = stream.OriginalStreamName;
                props.hash = stream.Hash;
                var linkedStreamIds = new List<string> {};
                props.linkedStreamIds = linkedStreamIds;
                if (stream.OriginalStreamName != stream.Name)
                {
                    linkedStreamIds.Add($"event-stream:{stream.OriginalStreamName}");
                }
            }
            else
            {
                var foundStreamName = doc.GetProperty<string>("name");
                var foundOriginalStreamName = doc.GetProperty<string>("originalStreamName");
                if (stream.Name != foundStreamName || stream.OriginalStreamName != foundOriginalStreamName)
                {
                    // We have a renamed stream
                    shouldSave = true;
                    props.documentType = "event-stream";
                    props.name = stream.Name;
                    props.originalStreamName = stream.OriginalStreamName;
                    props.hash = stream.Hash;
                    var linkedStreamIds = new List<string> { };
                    props.linkedStreamIds = linkedStreamIds;

                    //TODO: Implement this properly
                }
            }

            if (shouldSave)
            {
                doc.PutProperties(properties);
            }
            return Task.FromResult(streamId);
        }

        public async Task<PersistedRawEvent> GetEvent(Guid eventId)
        {
            var query = EventsByEventId.CreateQuery();
            query.StartKey = eventId;
            query.EndKey = eventId;
            query.Limit = 1;
            var results = await query.RunAsync();
            var row = results.SingleOrDefault();

            var evt = new PersistedRawEvent();
            dynamic props = row.AsJSONDictionary();
            evt.GlobalSequence = props.globalSequence;
            evt.EventID = props.eventId;
            evt.EventType = props.eventType;
            evt.PayloadFormat = props.payloadFormat;
            evt.UtcTimestamp = props.utcTimestamp;
            return evt;
        }

        private void InitializeViews()
        {
            var view = EventsView = Db.GetView("even-events");
            view.SetMap((doc, emit) =>
            {
                object documentType;
                if (doc.TryGetValue("documentType", out documentType))
                {
                    if (Equals(documentType, "event"))
                    {
                        emit(documentType, null);
                    }                    
                }
            }, "1");

            EventsByStreamName = view = Db.GetView("even-events-by-stream-name");
            view.SetMap((doc, emit) =>
            {
                object documentType;
                if (doc.TryGetValue("documentType", out documentType))
                {
                    if (Equals(documentType, "event"))
                    {
                        
                        object streamName;
                        if (doc.TryGetValue("streamName", out streamName))
                        {
                            emit(new[] {documentType, streamName}, null);
                        }
                    }                    
                }
            }, "1");


            EventsByEventId = view = Db.GetView("even-events-by-eventId");
            view.SetMap((doc, emit) =>
            {
                object documentType;
                if (doc.TryGetValue("documentType", out documentType))
                {
                    if (Equals(documentType, "event"))
                    {

                        object eventId;
                        if (doc.TryGetValue("eventId", out eventId))
                        {
                            emit(new[] { documentType, eventId }, null);
                        }
                    }
                }
            }, "1");


        }


    }
}