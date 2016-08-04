using System;
using Couchbase.Lite;
using Even.Persistence.CouchbaseLite.Testing;
using Even.Persistence.CouchbaseLite.Testing.Mocks;
using Ploeh.AutoFixture;
using Serilog;
using Specify;
using Xunit.Abstractions;

namespace Even.Persistence.CouchbaseLite.Internals
{
    public abstract class EventRepositoryScenario : Testing.ScenarioFor<EventRepository>
    {
        public ISequenceGenerator Sequence { get; private set; }
        public Database Db { get; private set; }  
        public ILogger Logger { get; private set;}        

        public virtual void Setup()
        {
            Logger = Log.ForContext(GetType());
            Sequence = new MockSequenceGenerator();
            Db = TestingUtility.GetDatabaseForCaller();
            Logger.Debug($"Database: {Db.Name} has {Db.GetDocumentCount()} documents.");
            Container.SystemUnderTest = new EventRepository(Db, Sequence);
        }

        public virtual void TearDown()
        {
            Logger.Debug($"Performing database cleanup for Database: {Db.Name}...");
            var results = Db.CreateAllDocumentsQuery().Run();
            foreach (var row in results)
            {
                Logger.Debug($"Deleting document {row.DocumentId}...");
                row.Document.Delete();
                Log.Debug($"Document {row.DocumentId} was deleted.");

            }
        }

        public override void SetContainer(IContainer container)
        {
            base.SetContainer(container);
        }

        public UnpersistedRawEvent CreateUnpersistedRawEvent(Stream stream, string eventType)
        {
            var fixture = new Fixture();
            return new UnpersistedRawEvent(
                fixture.Create<Guid>(),
                stream,
                eventType,
                fixture.Create<DateTimeOffset>().UtcDateTime,
                fixture.Create<byte[]>(),
                fixture.Create<byte[]>(),
                0
                );
        }

        public override void Specify()
        {
            var container = Container;
            base.Specify();
        }
    }
}