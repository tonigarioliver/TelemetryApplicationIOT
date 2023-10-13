using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelemetryApiRest.Services;
using TelemetryApiRest.Services.Implementation;

namespace TelemetryApp.Hubs
{
    public class DeviceMessageHub : Hub
    {
        private readonly IMqttListenerServiceRealTime mqttService;
        private readonly Dictionary<string, List<string>> clientTopics = new Dictionary<string, List<string>>();

        public DeviceMessageHub(IMqttListenerServiceRealTime mqttService)
        {
            this.mqttService = mqttService;
        }

        public override async Task OnConnectedAsync()
        {
            // Access topics from the connection query
            var topics = Context.GetHttpContext().Request.Query["topics"].ToArray();

            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");

            // Store the topics associated with the client
            clientTopics[Context.ConnectionId] = topics.ToList();

            foreach (var topic in topics)
            {
                mqttService.AddSubscription(topic);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (clientTopics.TryGetValue(Context.ConnectionId, out var topics))
            {
                foreach (var topic in topics)
                {
                    mqttService.RemoveSubscription(topic);
                }
                clientTopics.Remove(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task TestMe(string someRandomText)
        {
            await Clients.All.SendAsync($"{Context.User.Identity.Name} : {someRandomText}");
        }

        public async Task SendMessage(string machine, string telemetry)
        {
            await Clients.All.SendAsync("ReceiveMessage", machine, telemetry);
        }

        public async Task SendMqttMessage(string topic, string payload)
        {
            await Clients.All.SendAsync(topic, payload);
        }
    }
}
