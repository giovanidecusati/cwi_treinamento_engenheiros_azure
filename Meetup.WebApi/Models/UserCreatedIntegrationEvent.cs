using Meetup.WebApi.Infrastructure.IntegrationEvents;

namespace Meetup.WebApi.Models
{
    public class UserCreatedIntegrationEvent: IntegrationEvent
    {
        public string Name { get; }
        public string Email { get;}
        public string Password { get; }
        

        public UserCreatedIntegrationEvent(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;

        }
    }
}
