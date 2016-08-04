using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Even.Persistence.CouchbaseLite.Messages;

namespace Even.Persistence.CouchbaseLite.Internals
{
    public interface ISequenceGenerator
    {
        Task<long> Next(TimeSpan? timeout = null);
        Task<SequenceRange> ReserveRange(uint numberToReserve, TimeSpan? timeout = null);

    }

    public class DefaultSequenceGenerator : ISequenceGenerator
    {
        public DefaultSequenceGenerator(IActorRef sequence)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            Sequence = sequence;
        }

        internal IActorRef Sequence { get; }

        public async Task<long> Next(TimeSpan? timeout = null)
        {
            var reserved = await Sequence.Ask<SequenceNumbersReserved>(new ReserveSequenceNumbers(1), timeout);
            return reserved.End;
        }

        public async Task<SequenceRange> ReserveRange(uint numberToReserve, TimeSpan? timeout = null)
        {
            var reserved = await Sequence.Ask<SequenceNumbersReserved>(new ReserveSequenceNumbers(1), timeout);
            return SequenceRange.Create(reserved.Start, reserved.End);
        }
    }
}
