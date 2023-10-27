using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Client;
using MQTTnet;
using System.Text;
using IOTHistoricalDataService.Config;
using IOTHistoricalDataService.EventProcessing;
using IOTHistoricalDataService.Models;
using IOTHistoricalDataService.COR;

namespace IOTHistoricalDataService.Services
{
    internal class NewDataMQTTProcess
    {
        private readonly ILogger<NewDataMQTTProcess> logger;
        private readonly IMqttClient mqttClient;
        private readonly MQTTSettings mqttConfig;
        private bool isReconnecting;
        private CancellationTokenSource cancellationTokenSource;
        private MqttClientOptions mqttOptions; // Declare the mqttOptions field
        private List<string> topicsToListen = new List<string>();
        private readonly IMapper mapper;
        private readonly IEventProcessor eventProcessor;
        private readonly IServiceProvider serviceProvider;


        public NewDataMQTTProcess(IServiceProvider serviceProvider, IEventProcessor eventProcessor,IMapper mapper,
            IOptions<MQTTSettings> mqttConfig, ILogger<NewDataMQTTProcess> logger)
        {
            this.mqttConfig = mqttConfig.Value; // Use Value to get the current configuration
            this.logger = logger;
            this.eventProcessor = eventProcessor;
            this.mapper = mapper;
            this.serviceProvider = serviceProvider;
            mqttClient = new MqttFactory().CreateMqttClient();
            ConfigureMqttClient();
        }
        public virtual async Task<bool> ProcessMessageReceived(string topic, string payload)
        {

            try
            {
                var newMessageEvent = new MqttBackgroundMessageReceivedEvent(topic, payload);
                await eventProcessor.ProcessEventAsync(newMessageEvent);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public  async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Configure your MQTT client here, if needed

            try
            {
                // Connect to the MQTT broker
                // Your connection logic here
                ConnectAsync(); // You should implement ConnectAsync in your MqttListener
                using (var scope = serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    mapper.Map<List<DeviceModel>>(await unitOfWork.DeviceRepository.GetAllAsync())
                    .Select(d => d.SerialNumber)
                    .ToList()
                    .ForEach(topic => AddSubscription(topic));
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    // Listen for MQTT messages
                    // Retrieve MQTT messages and call the ProcessMessage method

                    // Delay for a period before checking for the next message
                    await Task.Delay(TimeSpan.FromSeconds(0.1), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to connect to MQTT broker: {ex.Message}");
                // Implement reconnection logic here
            }
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

            logger.LogInformation($"Received message on topic '{topic}': {message}");

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
                        logger.LogInformation("Reconnecting...");
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
                logger.LogInformation($"Connected as client{mqttOptions.ClientId}");

            }
            catch (Exception ex)
            {
                logger.LogError($"Connection failed: {ex.Message}");
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
            logger.LogInformation("Disconnected from MQTT broker.");
            // Implement reconnection logic here
            StartReconnect();
        }
        public async Task<bool> AddSubscription(string topic)
        {
            try
            {
                if (!topicsToListen.Contains(topic))
                {
                    await mqttClient.SubscribeAsync(topic);
                    topicsToListen.Add(topic);
                    return true;
                }
                return false; // Topic is already subscribed
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to subscribe to topic '{topic}': {ex.Message}");
                return false;
            }
        }


        public async Task<bool> RemoveSubscription(string topic)
        {
            try
            {
                if (topicsToListen.Contains(topic))
                {
                    await mqttClient.UnsubscribeAsync(topic);
                    topicsToListen.Remove(topic);
                    return true;
                }
                return false; // Topic is not currently subscribed
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to unsubscribe from topic '{topic}': {ex.Message}");
                return false;
            }
        }
    }
}
