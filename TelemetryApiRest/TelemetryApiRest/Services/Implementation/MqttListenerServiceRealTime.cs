using Microsoft.Extensions.Options;
using MQTTnet.Client;
using MQTTnet;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TelemetryApiRest.Config;
using TelemetryApiRest.Services;
using TelemetryApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic; // Add this for List
using TelemetryApiRest.Helper;
using TelemetryApiRest.EventProcessing;

namespace TelemetryApiRest.Services.Implementation
{
    public class MqttListenerServiceRealTime : MqttListener, IMqttListenerServiceRealTime
    {
        private readonly ILogger<MqttListenerServiceRealTime> _logger;
        private readonly IHubContext<DeviceMessageHub> hubContext;
        private readonly IEventProcessor eventProcessor;

        public MqttListenerServiceRealTime(IEventProcessor eventProcessor,
            IHubContext<DeviceMessageHub> hubContext, ILogger<MqttListenerServiceRealTime> logger,
            IOptions<MQTTSettings> mqttConfig, ILogger<MqttListener> listenerlogger)
            :base(mqttConfig,listenerlogger)
        {
            this.hubContext = hubContext;
            this.eventProcessor=eventProcessor;
            _logger = logger;
            base.ConnectAsync();
        }

        public override async Task<bool> ProcessMessageReceived(string topic, string payload)
        {
            // Your custom implementation for ProcessMessage in MqttListenerServiceRealTime
            // You can call the base.ProcessMessage method if needed.
            try
            {
                var newMessageEvent=new RealTimeMessageReceivedEvent(topic, payload);
                await eventProcessor.ProcessEventAsync(newMessageEvent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send message on topic '{topic}': {ex.Message}");
                return false;
            }
        }
    }
}
