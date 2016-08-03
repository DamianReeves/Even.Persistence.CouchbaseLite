using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Even.Persistence.CouchbaseLite.Messages
{
    [Equals]
    public class SequenceIncremented
    {
        public SequenceIncremented(long counter)
        {
            Counter = counter;
        }

        public long Counter { get; }
    }

    [Equals]
    public class SequenceNumbersReserved
    {

        public SequenceNumbersReserved(long start, long end)
        {
            Start = start;
            End = end;
        }

        public long Start { get; }
        public long End { get; }
    }

    [Equals]
    public class SequenceNumberReservationRejected
    {
        public SequenceNumberReservationRejected(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; }
    }

    [Equals]
    internal class SequenceStarted
    {
        public SequenceStarted(long next)
        {
            Next = next;
        }

        public long Next { get; }
    }
}
