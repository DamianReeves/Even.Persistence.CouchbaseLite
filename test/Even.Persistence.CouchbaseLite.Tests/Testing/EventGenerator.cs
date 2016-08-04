using System.Linq;

namespace Even.Persistence.CouchbaseLite.Testing
{
    public static class EventGenerator
    {
        public static UnpersistedRawEvent CreateEventFor<TDomainEvent>(TDomainEvent domainEvent, string streamName = null)
        {
            streamName = streamName ?? typeof(TDomainEvent).Name;
            var unpersistedEvent = new UnpersistedEvent(new Stream(streamName), domainEvent);
            var events = UnpersistedRawEvent.FromUnpersistedEvents(new[] {unpersistedEvent}, new DefaultSerializer());
            return events.First();
        }
    }
}