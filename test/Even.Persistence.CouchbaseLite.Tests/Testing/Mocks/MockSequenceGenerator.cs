using System;
using System.Threading.Tasks;
using Even.Persistence.CouchbaseLite.Internals;

namespace Even.Persistence.CouchbaseLite.Testing.Mocks
{
    public class MockSequenceGenerator : ISequenceGenerator
    {
        private long _counter;
        public MockSequenceGenerator(long next = 1)
        {
            if (next < 0) next = 1;
            _counter = next;
        }
        public Task<long> Next(TimeSpan? timeout = null)
        {
            return Task.FromResult(_counter++);
        }

        public Task<SequenceRange> ReserveRange(uint numberToReserve, TimeSpan? timeout = null)
        {
            var start = _counter;
            _counter = _counter + numberToReserve;
            var end = _counter - 1;
            return Task.FromResult(SequenceRange.Create(start, end));
        }
    }
}