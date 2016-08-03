using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Even.Persistence.CouchbaseLite.Messages
{
    internal class StartSequence
    {
        public static readonly StartSequence Instance = new StartSequence();
    }

    [Equals]
    public class IncrementSequence
    {
        public IncrementSequence(uint increment)
        {
            Increment = increment;
        }

        public uint Increment { get; }
    }

    [Equals]
    public class ReserveSequenceNumbers
    {
        public ReserveSequenceNumbers(uint numberToReserve)
        {
            NumberToReserve = numberToReserve;
        }

        public uint NumberToReserve { get; }
    }
}
