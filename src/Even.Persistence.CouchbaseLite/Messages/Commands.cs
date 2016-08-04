using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace Even.Persistence.CouchbaseLite.Messages
{
    internal class StartSequence
    {
        public static readonly StartSequence Instance = new StartSequence();
    }

    [Equals]
    internal class IncrementSequence
    {
        public IncrementSequence(uint increment)
        {
            Increment = increment;
        }

        public uint Increment { get; }
    }

    [Equals]
    internal class ReserveSequenceNumbers
    {
        public ReserveSequenceNumbers(uint numberToReserve)
        {
            NumberToReserve = numberToReserve;
        }

        public uint NumberToReserve { get; }
    }

    public class Observe
    {
        public Observe(IActorRef observer)
        {
            Observer = observer;
        }
         
        public IActorRef Observer { get; }
    }

    internal class StartStore
    {
        public static readonly StartStore Instance = new StartStore();
    }

    [Equals]
    internal class WriteEvents
    {        
        public WriteEvents(IReadOnlyCollection<IUnpersistedRawStreamEvent> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            Events = events;
        }

        public IReadOnlyCollection<IUnpersistedRawStreamEvent> Events { get; }

    }
}
