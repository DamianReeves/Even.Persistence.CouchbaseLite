namespace Even.Persistence.CouchbaseLite.Internals
{
    [Equals]
    public class SequenceRange
    {
        public SequenceRange(long start, long end)
        {
            Start = start;
            End = end;
        }

        public long Start { get; }
        public long End { get; }

        public static SequenceRange Create(long inclusiveBoundary1, long inclusiveBoundary2)
        {
            if (inclusiveBoundary1 <= inclusiveBoundary2)
            {
                return new SequenceRange(inclusiveBoundary1, inclusiveBoundary2);
            }

            return new SequenceRange(inclusiveBoundary2, inclusiveBoundary1);
        }
    }
}