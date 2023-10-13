using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MQTTnet.Client;
using MQTTnet;
using System.Text;
using TelemetryApiRest.Config;
using TelemetryApiRest.Services;
using TelemetryApp.Hubs;

namespace TelemetryApiRest.Helper
{
    public class MqttListener : IMqttListener
    {
        private readonly IMqttClient mqttClient;
        private readonly MQTTSettings mqttConfig;
        private readonly ILogger<MqttListener> _logger;
        private bool isReconnecting;
        private CancellationTokenSource cancellationTokenSource;
        private MqttClientOptions mqttOptions; // Declare the mqttOptions field
        private List<string> subscribedTopics = new List<string>(); // Store the subscribed topics


        public MqttListener(IOptions<MQTTSettings> mqttConfig, ILogger<MqttListener> logger)
        {
            this.mqttConfig = mqttConfig.Value; // Use Value to get the current configuration
            _logger = logger;
            mqttClient = new MqttFactory().CreateMqttClient();
            _logger = logger;
            ConfigureMqttClient();
        }

        private void ConfigureMqttClient()
        {
            mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttConfig.Host, mqttConfig.Port)
                .WithClientId(Guid.NewGuid().ToString()) // Generate a unique client ID
                .WithCredentials(mqttConfig.Username, mqttConfig.Password)
                .WithCleanSession()
                .Build();
            mqttClient.ConnectedAsync += HandleConnectedAsync;
            mqttClient.DisconnectedAsync += HandleDisconnectedAsync;
            mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;
        }

        // In your MqttListenerService, invoke the SignalR hub method when a new MQTT message arrives.
        public Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            string topic = eventArgs.ApplicationMessage.Topic;
            string message = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);

            _logger.LogInformation($"Received message on topic '{topic}': {message}");

            // Process the MQTT message and then send it to SignalR clients.
            ProcessMessageReceived(topic, message).Wait();

            return Task.CompletedTask;
        }

        private void StartReconnect()
        {
            if (!isReconnecting)
            {
                isReconnecting = true;
                cancellationTokenSource = new CancellationTokenSource();

                Task.Run(async () =>
                {
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        _logger.LogInformation("Reconnecting...");
                        ConnectAsync();
                        await Task.Delay(TimeSpan.FromSeconds(10), cancellationTokenSource.Token);
                    }
                    isReconnecting = false;
                }, cancellationTokenSource.Token);
            }
        }

        public void ConnectAsync()
        {
            try
            {
                mqttClient.ConnectAsync(mqttOptions).Wait();
                _logger.LogInformation($"Connected as client{mqttOptions.ClientId}");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Connection failed: {ex.Message}");
                // Implement reconnection logic here
                StartReconnect();
            }
        }

        public async Task StopAsync()
        {
            if (isReconnecting)
            {
                cancellationTokenSource.Cancel();
            }

            await mqttClient.DisconnectAsync();
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            isReconnecting = false;
        }

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            _logger.LogInformation("Disconnected from MQTT broker.");
            // Implement reconnection logic here
            StartReconnect();
        }
        public async Task<bool> AddSubscription(string topic)
        {
            try
            {
                if (!subscribedTopics.Contains(topic))
                {
                    await mqttClient.SubscribeAsync(topic);
                    subscribedTopics.Add(topic);
                    return true;
                }
                return false; // Topic is already subscribed
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to subscribe to topic '{topic}': {ex.Message}");
                return false;
            }
        }


        public async Task<bool> RemoveSubscription(string topic)
        {
            try
            {
                if (subscribedTopics.Contains(topic))
                {
                    await mqttClient.UnsubscribeAsync(topic);
                    subscribedTopics.Remove(topic);
                    return true;
                }
                return false; // Topic is not currently subscribed
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to unsubscribe from topic '{topic}': {ex.Message}");
                return false;
            }
        }

        public virtual async Task<bool> ProcessMessageReceived(string topic, string payload)
        {

            try
            {
                return true; // Topic is not currently subscribed
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
