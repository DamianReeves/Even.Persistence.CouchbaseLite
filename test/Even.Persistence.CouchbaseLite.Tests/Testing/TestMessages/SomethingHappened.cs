namespace Even.Persistence.CouchbaseLite.Testing.TestMessages
{
    public class SomethingHappened
    {
        public SomethingHappened(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
