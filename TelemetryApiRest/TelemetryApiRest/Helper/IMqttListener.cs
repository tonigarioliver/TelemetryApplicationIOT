using MQTTnet.Client;

namespace TelemetryApiRest.Helper
{
    public interface IMqttListener
    {
        Task<bool> AddSubscription(string topic);
        Task<bool> RemoveSubscription(string topic);
        Task<bool> ProcessMessageReceived(string topic, string payload);
        void ConnectAsync();
    }
}
