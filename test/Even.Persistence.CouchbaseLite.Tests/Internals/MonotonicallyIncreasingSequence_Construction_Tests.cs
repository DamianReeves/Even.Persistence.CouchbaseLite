using Akka.TestKit.Xunit2;
using Even.Persistence.CouchbaseLite.Messages;
using Even.Persistence.CouchbaseLite.Testing;
using FluentAssertions;
using Xunit;

namespace Even.Persistence.CouchbaseLite.Internals
{
    public class MonotonicallyIncreasingSequence_Construction_Tests : TestKit
    {        

        [Fact]
        public void A_null_database_causes_the_sequence_to_be_faulted()
        {
            var sequence = ActorOfAsTestActorRef<MonotonicallyIncreasingSequence>(MonotonicallyIncreasingSequence.CreateProps(null));
            sequence.UnderlyingActor.IsFaulted.Should().BeTrue();
        }

        [Fact]
        public void Database_passed_in_should_be_assigned_to_Db_property()
        {
            using (var db = TestingUtility.GetDatabaseForCaller())
            {
                var sequence = new MonotonicallyIncreasingSequence(db);
                sequence.Db.Should().Be(db);
            }                
        }

        [Fact]
        public void The_next_sequence_number_for_a_new_database_should_be_1()
        {
            using (var db = TestingUtility.GetIsolatedDatabaseForCaller())
            {
                Sys.EventStream.Subscribe(TestActor, typeof(SequenceStarted));
                var sequence =
                    ActorOfAsTestActorRef<MonotonicallyIncreasingSequence>(MonotonicallyIncreasingSequence.CreateProps(db), "sequence");

                ExpectMsg(new SequenceStarted(1));
                db.Delete();
            }                
        }
    }     
}
