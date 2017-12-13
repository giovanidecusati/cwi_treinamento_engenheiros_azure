using System.Threading.Tasks;

namespace Meetup.WebApi.Infrastructure.IntegrationEvents
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event) where T : IntegrationEvent;
    }
}
