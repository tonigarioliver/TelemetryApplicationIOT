using Xunit;
using Moq;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using MQTTnet;
using TelemetryApiRest.Config;
using MQTTnet.Packets;



namespace TelemetryApiRest.Helper.Tests
{
    public class MqttListenerTests
    {
        [Fact]
        public async Task ProcessMessageReceived_ReceivedMessage_ReturnsTrue()
        {
            var loggerMockListener = new Mock<ILogger<MqttListener>>();
            var mqttConfig = new MQTTSettings
            {
                Host = "tom.uib.es",
                Port = 1883, // Update with your MQTT broker port
                Username = "yourUsername",
                Password = "yourPassword"
            };

            var mqttListener = new MqttListener(
                Options.Create(mqttConfig),
                loggerMockListener.Object // Use MockedLogger
            );

            var topic = "testTopic";
            var payload = "testPayload";

            // Act
            var result = await mqttListener.ProcessMessageReceived(topic, payload);

            // Assert
            Assert.True(result);
        }
        
    }
}
