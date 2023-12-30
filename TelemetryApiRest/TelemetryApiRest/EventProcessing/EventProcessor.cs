using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.Text.Json;
using TelemetryApiRest.COR;
using TelemetryApiRest.Entity;
using TelemetryApiRest.Services;
using TelemetryApiRest.Services.Implementation;
using TelemetryApp.Hubs;

namespace TelemetryApiRest.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IHubContext<DeviceMessageHub> hubContext;


        public EventProcessor(IServiceProvider serviceProvider, IHubContext<DeviceMessageHub> hubContext)
        {
            this.serviceProvider = serviceProvider;
            this.hubContext = hubContext;
        }
        public async Task ProcessEventAsync(EventBase @event)
        {
            var rabbitMQManager = serviceProvider.GetRequiredService<RabbitMQManager>();
            switch (@event.EventType)
            {
                case EventType.MqttBackgroundMessageReceived:
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        Device exisitngDevice = await unitOfWork.DeviceRepository
                            .GetAsync(d => d.SerialNumber == ((MqttBackgroundMessageReceivedEvent)@event).deviceSerialNumber);
                        if (exisitngDevice == null)
                        {
                           var mqttRealTimeService = serviceProvider.GetService<IMqttListenerServiceRealTime>();
                           await mqttRealTimeService.RemoveSubscription(((MqttBackgroundMessageReceivedEvent)@event).deviceSerialNumber);
                        }
                        else
                        {
                            DeviceRecord newRecord = new DeviceRecord
                            {
                                device = exisitngDevice,
                                lastRecord = ((MqttBackgroundMessageReceivedEvent)@event).message
                            };

                            await unitOfWork.DeviceRecordsRepository.CreateAsync(newRecord);
                            await unitOfWork.CompleteAsync();
                        }
                    }
                    break;
                case EventType.DeleteDevice:
                    if (rabbitMQManager != null)
                    {
                        rabbitMQManager.PublishMessage(exchange: "", routingKey: "device", messageBody: Encoding.UTF8.GetBytes("delete"));
                    }
                    break;

                case EventType.AddDevice:
                    if (rabbitMQManager != null)
                    {
                        rabbitMQManager.PublishMessage(exchange: "", routingKey: "device", messageBody: Encoding.UTF8.GetBytes("add"));
                    }
                    break;

                case EventType.RealTimeMessageReceived:      
                    string topic= ((RealTimeMessageReceivedEvent)@event).deviceSerialNumber;
                    string device= ((RealTimeMessageReceivedEvent)@event).deviceSerialNumber;
                    string payload = ((RealTimeMessageReceivedEvent)@event).message;
                    await hubContext.Clients.All
                        .SendAsync(topic, device,payload);
                    break;
            }

        }
    }
}
