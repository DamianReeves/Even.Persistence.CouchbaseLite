using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Couchbase.Lite;
using Even.Persistence.CouchbaseLite.Messages;
using Even.Persistence.CouchbaseLite.Testing;
using FluentAssertions;
using Ploeh.AutoFixture;
using Xunit;

namespace Even.Persistence.CouchbaseLite
{
    public class MonotonicallyIncreasingSequenceSpecs : TestKit
    {
        public Database Db { get; }
        public MonotonicallyIncreasingSequenceSpecs()
        {
            Db = TestingUtility.GetIsolatedDatabaseForCaller();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Delete();
            }
            base.Dispose(disposing);
        }

        [Fact]
        public void Incrementing_the_counter_should_cause_value_to_increment_by_the_provided_increment()
        {
            var fixture = new Fixture();
            var increment = fixture.Create<uint>();
            var sequence =
                ActorOfAsTestActorRef<MonotonicallyIncreasingSequence>(MonotonicallyIncreasingSequence.CreateProps(Db));
            var startingValue = sequence.UnderlyingActor.Next;
            sequence.Tell(new IncrementSequence(increment));
            sequence.UnderlyingActor.Next.Should().Be(startingValue + increment);

        }

        [Fact]
        public void Reserving_zero_sequence_numbers_should_result_in_rejection()
        {
            var sequence =
                ActorOfAsTestActorRef<MonotonicallyIncreasingSequence>(MonotonicallyIncreasingSequence.CreateProps(Db));

            sequence.Tell(new ReserveSequenceNumbers(0));

            ExpectMsg(new SequenceNumberReservationRejected("Cannot reserve 0 sequences. The number to reserve must be greater than 0."));
        }



        [Fact]
        public void Reserving_sequence_numbers_should_increment_the_counter_and_reserve_the_appropriate_range()
        {
            var fixture = new Fixture();
            var numToReserve = fixture.Create<uint>() + 1;
            var sequence =
                ActorOfAsTestActorRef<MonotonicallyIncreasingSequence>(MonotonicallyIncreasingSequence.CreateProps(Db));

            var startingValue = sequence.UnderlyingActor.Next;


            sequence.Tell(new ReserveSequenceNumbers(numToReserve));

            var expectedEnd = startingValue + numToReserve;
            var expectedStart = startingValue + 1;

            var expectedMessage = new SequenceNumbersReserved(expectedStart, expectedEnd);

            ExpectMsg(expectedMessage);

            sequence.UnderlyingActor.Next.Should().Be(startingValue + numToReserve);
        }
    }
}