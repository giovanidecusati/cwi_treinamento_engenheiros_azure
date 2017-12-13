using Meetup.FunctionApp.Infrastructure;
using Meetup.FunctionApp.Infrastructure.Data;
using Meetup.FunctionApp.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;

namespace Meetup.FunctionApp
{
    public static class RegisteredUserEventFunction
    {
        private static string _key = TelemetryConfiguration.Active.InstrumentationKey = FunctionHelper.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
        private static TelemetryClient _telemetry = new TelemetryClient() { InstrumentationKey = _key };

        [FunctionName("RegisteredUserEventFunction")]
        public static void Run(
            [ServiceBusTrigger("usercreatedintegrationevent", AccessRights.Listen)]
            string myQueueItem, TraceWriter log)
        {
            _telemetry.TrackEvent($"RegisteredUserEventFunction :: Function called: {myQueueItem}");

            log.Info($"C# ServiceBus topic trigger function start processing message: {myQueueItem}");

            try
            {
                var userCreatedIntegrationEvent = JsonConvert.DeserializeObject<UserCreatedIntegrationEvent>(myQueueItem);
                using (var userRepository = new UserRepository())
                {
                    var user = new User(userCreatedIntegrationEvent.Name, userCreatedIntegrationEvent.Email, userCreatedIntegrationEvent.Password);
                    if (user.IsValid())
                        userRepository.Create(user);
                    else
                        log.Info($"C# ServiceBus topic trigger function error: UserInvalid");
                }
            }
            catch(Exception e)
            {
                _telemetry.TrackException(e);
            }

            log.Info($"C# ServiceBus topic trigger function processed message.");
        }
    }
}
