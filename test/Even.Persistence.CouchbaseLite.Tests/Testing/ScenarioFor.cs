using Specify.Stories;
using Xunit;

namespace Even.Persistence.CouchbaseLite.Testing
{
    public abstract class ScenarioFor<TSut> : Specify.ScenarioFor<TSut>
        where TSut : class
    {
        [Fact]        // xUnit
        public override void Specify()
        {
            base.Specify();
        }
    }
    public abstract class ScenarioFor<TSut, TStory> : Specify.ScenarioFor<TSut, TStory>
        where TSut : class
        where TStory : Story, new()
    {
        [Fact]        // xUnit
        public override void Specify()
        {
            base.Specify();
        }
    }
}