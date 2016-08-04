using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Lite;
using Even.Persistence.CouchbaseLite.Testing;
using Even.Persistence.CouchbaseLite.Testing.TestMessages;
using FluentAssertions;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Abstractions;

namespace Even.Persistence.CouchbaseLite.Internals
{
    public class EventRepository_when_writing_an_event_with_a_sequence_number_of_0 : EventRepositoryScenario
    {
        public IUnpersistedRawEvent Event { get; set; }
        public long ReturnedGlobalSequenceNumber { get; set; }

        
        public void Given_an_event_with_an_unassigned_global_sequence_number()
        {
            var fixture = new Fixture();
            var domainEvent = fixture.Create<CounterIncremented>();
            Event = EventGenerator.CreateEventFor(domainEvent, "Counter");
            Event.GlobalSequence = 0;
        }

        public async Task When_the_event_is_written()
        {
            ReturnedGlobalSequenceNumber = await SUT.WriteEvent(Event);
        }

        public void Then_the_global_sequence_number_should_have_been_assigned()
        {
            Event.GlobalSequence.Should().NotBe(0);
        }

        public void And_the_global_sequence_number_returned_should_match_the_one_assigned()
        {
            Event.GlobalSequence.Should().Be(ReturnedGlobalSequenceNumber);
        }

        public void And_the_document_should_have_been_written_to_the_Db()
        {
            var document = Db.GetExistingDocument($"event:{Event.EventID}");
            document.Should().NotBeNull();
            Logger.Debug("Returned document with properties {@properties}", document.Properties);
            Logger.Debug("Returned document with user properties {@userProperties}", document.UserProperties);
        }

        public void And_the_EventID_should_have_been_saved()
        {
            var document = Db.GetExistingDocument($"event:{Event.EventID}");
            document.GetProperty<string>("eventId").Should().Be(Event.EventID.ToString());
        }
    }
}
