using System.Threading.Tasks;
using Even.Persistence.CouchbaseLite.Testing;
using Even.Persistence.CouchbaseLite.Testing.TestMessages;
using FluentAssertions;
using Ploeh.AutoFixture;

namespace Even.Persistence.CouchbaseLite.Internals
{
    public class EventRepository_when_getting_an_event_that_was_saved_by_event_id : EventRepositoryScenario
    {
        public IUnpersistedRawEvent Event { get; set; }
        public PersistedRawEvent RetrievedEvent { get; set; }
        public async Task Given_a_saved_event()
        {
            var fixture = new Fixture();
            var domainEvent = fixture.Create<CounterIncremented>();
            Event = EventGenerator.CreateEventFor(domainEvent);
            var seq = SUT.WriteEvent(Event);
        }

        public async Task When_we_get_the_event_by_id()
        {
            RetrievedEvent = await SUT.GetEvent(Event.EventID);
            Logger.Debug("Retrieved an event: {@RetrievedEvent}", RetrievedEvent);

            var document = Db.GetExistingDocument($"event:{Event.EventID}");
            document.Should().NotBeNull();
            Logger.Debug("Returned document with properties {@properties}", document.Properties);
            Logger.Debug("Returned document with user properties {@userProperties}", document.UserProperties);
        }

        public void Then_the_retrieved_event_should_not_be_null()
        {
            RetrievedEvent.Should().NotBeNull();
        }

        public void And_the_payload_should_be_the_payload_that_was_saved()
        {
            RetrievedEvent.Payload.Should().Equal(Event.Payload);
        }
    }
}