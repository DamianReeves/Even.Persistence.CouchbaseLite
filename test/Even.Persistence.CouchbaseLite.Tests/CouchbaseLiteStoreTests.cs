using System.Data;
using Couchbase.Lite;
using Even.Persistence.CouchbaseLite.Internals;
using Even.Persistence.CouchbaseLite.Testing;
using Ploeh.AutoFixture;

namespace Even.Persistence.CouchbaseLite
{
    public class CouchbaseLiteStoreTests : EventStoreTests
    {
        public string DbName { get; private set; }
        public Database Db { get; private set; }
        public ISequenceGenerator Sequence { get; private set; }

        public CouchbaseLiteStoreTests()
        {

        }
        protected override IEventStore CreateStore()
        {
            var dbName = $"even-tests-{nameof(CouchbaseLiteStoreTests)}".ToLowerInvariant();

            var db = Manager.SharedInstance.GetDatabase(dbName);
            var sequence = Sys.ActorOf(MonotonicallyIncreasingSequence.CreateProps(db));
            Sequence = new DefaultSequenceGenerator(sequence);
            return new CouchbaseLiteStore(Sys, db, Sequence);
        }

        protected override void ResetStore()
        {
            CouchbaseLiteStore.DeleteEventStoreDocuments(Db);
        }
    }
}