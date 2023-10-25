using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using TelemetryApiRest.Config;
using TelemetryApiRest.EventProcessing;
using TelemetryApiRest.Helper;
using TelemetryApiRest.Services.Implementation;
using TelemetryApp.Hubs;


namespace TelemetryApiRestUnitTest.Services
{
    public class MqttListenerServiceRealTimeTests
    {
        [Fact]
        public async Task ProcessMessageReceived_ValidMessage_ReturnsTrue()
        {
            // Arrange
            var eventProcessorMock = new Mock<IEventProcessor>();
            var hubContextMock = new Mock<IHubContext<DeviceMessageHub>>();
            var loggerMock = new Mock<ILogger<MqttListenerServiceRealTime>>();
            var loggerMockListener = new Mock<ILogger<MqttListener>>();
            var mqttConfig = new MQTTSettings
            {
                Host = "tom.uib.es",
                Port = 1883, // Update with your MQTT broker port
                Username = "yourUsername",
                Password = "yourPassword"
            };

            var mqttListenerService = new MqttListenerServiceRealTime(
                eventProcessorMock.Object,
                hubContextMock.Object,
                loggerMock.Object,
                Options.Create(mqttConfig),
                loggerMockListener.Object // Use MockedLogger
            );

            var topic = "testTopic";
            var payload = "testPayload";

            eventProcessorMock.Setup(e => e.ProcessEventAsync(It.IsAny<RealTimeMessageReceivedEvent>()))
                .Returns(Task.FromResult(true));

            // Act
            var result = await mqttListenerService.ProcessMessageReceived(topic, payload);

            // Assert
            eventProcessorMock.Verify(e => e.ProcessEventAsync(It.Is<RealTimeMessageReceivedEvent>(evt =>
                evt.deviceSerialNumber == topic && evt.message == payload)), Times.Once);

            Assert.True(result);
        }

        [Fact]
        public async Task ProcessMessageReceived_ExceptionOccurs_ReturnsFalse()
        {
            // Arrange
            var eventProcessorMock = new Mock<IEventProcessor>();
            var hubContextMock = new Mock<IHubContext<DeviceMessageHub>>();
            var loggerMock = new Mock<ILogger<MqttListenerServiceRealTime>>();
            var loggerMockListener = new Mock<ILogger<MqttListener>>();
            var mqttConfig = new MQTTSettings
            {
                Host = "tom.uib.es",
                Port = 1883, // Update with your MQTT broker port
                Username = "yourUsername",
                Password = "yourPassword"
            };

            var mqttListenerService = new MqttListenerServiceRealTime(
                eventProcessorMock.Object,
                hubContextMock.Object,
                loggerMock.Object,
                Options.Create(mqttConfig),
                loggerMockListener.Object // Use MockedLogger
            );
            var topic = "testTopic";
            var payload = "testPayload";

            eventProcessorMock.Setup(e => e.ProcessEventAsync(It.IsAny<RealTimeMessageReceivedEvent>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await mqttListenerService.ProcessMessageReceived(topic, payload);

            // Assert
            eventProcessorMock.Verify(e => e.ProcessEventAsync(It.Is<RealTimeMessageReceivedEvent>(evt =>
                evt.deviceSerialNumber == topic && evt.message == payload)), Times.Once);
            Assert.False(result);
            loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}
