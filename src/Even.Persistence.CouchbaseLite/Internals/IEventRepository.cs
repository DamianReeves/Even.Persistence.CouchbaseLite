using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Even.Persistence.CouchbaseLite.Internals
{
    public interface IEventRepository
    {
        Task<long> WriteEvent(IUnpersistedRawEvent @event);
        Task<PersistedRawEvent> GetEvent(Guid eventId);
    }
}
