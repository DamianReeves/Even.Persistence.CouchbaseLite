namespace Even.Persistence.CouchbaseLite.Testing.TestMessages
{
    [Equals]
    public class CounterIncremented
    {
        public CounterIncremented(long value)
        {
            Value = value;
        }

        public long Value { get; }
    }
}