using Meetup.WebApi.Infrastructure.Data;
using Meetup.WebApi.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Meetup.WebApi.Infrastructure.IntegrationEvents
{
    public class AzureEventBus : IEventBus
    {
        readonly IQueueClient _queueClient;
        readonly ILogger<AzureEventBus> _logger;
        readonly IServiceProvider _serviceProvider;

        public AzureEventBus(IOptions<Eventconfiguration> options, ILogger<AzureEventBus> logger, IServiceProvider serviceProvider)
        {
            if (options.Value == null)
                throw new ArgumentNullException(nameof(options));

            _queueClient = new QueueClient(options.Value.ConnectrionString, options.Value.QueueName);
            _logger = logger;
            _serviceProvider = serviceProvider;
            RegisterOnMessageHandlerAndReceiveMessages();
        }

        public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
        {
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)));

            _logger.LogInformation("Sending message");
            _logger.LogInformation(String.Format("Message id: {0}", message.MessageId));
            _logger.LogInformation("End sending message");

            await _queueClient.SendAsync(message);
        }

        void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False value below indicates the Complete will be handled by the User Callback as seen in `ProcessMessagesAsync`.
                AutoComplete = false
            };

            // Register the function that will process messages
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var meetupDbcontext = scope.ServiceProvider.GetService<MeetupDbcontext>())
                {
                    // Process the message
                    _logger.LogInformation($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

                    var userCreatedIntegrationEvent = JsonConvert.DeserializeObject<UserCreatedIntegrationEvent>(Encoding.UTF8.GetString(message.Body));

                    await meetupDbcontext.Users.AddAsync(new User(userCreatedIntegrationEvent.Name, userCreatedIntegrationEvent.Email, userCreatedIntegrationEvent.Password));
                    await meetupDbcontext.SaveChangesAsync();

                    // Complete the message so that it is not received again.
                    // This can be done only if the queueClient is opened in ReceiveMode.PeekLock mode (which is default).
                    await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
                }
            }
        }

        // Use this Handler to look at the exceptions received on the MessagePump
        Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogInformation($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }
    }
}
