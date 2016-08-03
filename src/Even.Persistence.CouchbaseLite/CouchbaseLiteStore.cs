using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Couchbase.Lite;

namespace Even.Persistence.CouchbaseLite
{
    public class CouchbaseLiteStore : IEventStore
    {
        public static readonly string EventsDocumentId = "___even-cbl-store-events";
        public static readonly string ProjectionsDocumentId = "___even-cbl-store-projections";
        public static readonly string GlobalsDocumentId = "___even-cbl-store-globals";

        public CouchbaseLiteStore(ActorSystem actorSystem, Database db)
        {
            if (actorSystem == null) throw new ArgumentNullException(nameof(actorSystem));
            if (db == null) throw new ArgumentNullException(nameof(db));
            Db = db;
        }

        public Database Db { get; }
        public ActorSystem Sys { get; }

        public Task InitializeAsync()
        {
            var doc = Db.GetDocument("even-global-state");
            return Task.FromResult(0);
        }

        #region IEventStoreWriter
        public Task WriteAsync(IReadOnlyCollection<IUnpersistedRawStreamEvent> events)
        {
            throw new NotImplementedException();
        }

        public Task WriteStreamAsync(Stream stream, int expectedSequence, IReadOnlyCollection<IUnpersistedRawEvent> events)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IEventStoreReader
        public Task<long> ReadHighestGlobalSequenceAsync()
        {
            throw new NotImplementedException();
        }

        public Task ReadAsync(long initialSequence, int count, Action<IPersistedRawEvent> readCallback, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task ReadStreamAsync(Stream stream, int initialSequence, int count, Action<IPersistedRawEvent> readCallback, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IProjectionStoreWriter
        public Task ClearProjectionIndexAsync(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task WriteProjectionIndexAsync(Stream stream, int expectedSequence, IReadOnlyCollection<long> globalSequences)
        {
            throw new NotImplementedException();
        }

        public Task WriteProjectionCheckpointAsync(Stream stream, long globalSequence)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IProjectionStoreReader
        public Task<long> ReadProjectionCheckpointAsync(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task<long> ReadHighestIndexedProjectionGlobalSequenceAsync(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task<int> ReadHighestIndexedProjectionStreamSequenceAsync(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task ReadIndexedProjectionStreamAsync(Stream stream, int initialSequence, int count, Action<IPersistedRawEvent> readCallback,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }        

        #endregion

        internal void DeleteEventStoreDocuments()
        {
            DeleteEventStoreDocuments(Db);
        }

        internal static void DeleteEventStoreDocuments(Database db)
        {
            var doc = db.GetExistingDocument(EventsDocumentId);
            doc?.Delete();

            doc = db.GetExistingDocument(ProjectionsDocumentId);
            doc?.Delete();

            doc = db.GetExistingDocument(GlobalsDocumentId);
            doc?.Delete();
        }
    }
}
