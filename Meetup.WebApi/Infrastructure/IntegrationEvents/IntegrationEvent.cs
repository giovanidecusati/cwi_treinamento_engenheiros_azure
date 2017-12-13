using System;

namespace Meetup.WebApi.Infrastructure.IntegrationEvents
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Date = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public DateTime Date { get; }
        public Guid Id { get; }

    }
}