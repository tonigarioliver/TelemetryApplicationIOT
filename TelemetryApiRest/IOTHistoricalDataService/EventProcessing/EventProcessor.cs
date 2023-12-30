
using IOTHistoricalDataService.COR;
using IOTHistoricalDataService.Entity;
using IOTHistoricalDataService.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;


namespace IOTHistoricalDataService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceProvider serviceProvider;


        public EventProcessor(IServiceProvider serviceProvider) { 
            this.serviceProvider = serviceProvider;
        }
        public async Task ProcessEventAsync(EventBase @event)
        {
            var mqttService = serviceProvider.GetService<NewDataMQTTProcess>();
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
                            mqttService.RemoveSubscription(((MqttBackgroundMessageReceivedEvent)@event).deviceSerialNumber);
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
                    if (mqttService != null)
                    {
                        await mqttService.RemoveSubscription(((DeleteDevice)@event).deviceSerialNumber);
                    }
                    break;

                case EventType.AddDevice:
                    if (mqttService != null)
                    {
                        await mqttService.AddSubscription(((AddDevice)@event).deviceSerialNumber);
                    }
                    break;
            }
        }
    }
}
