namespace TelemetryApiRest.EventProcessing
{
    public class RealTimeMessageReceivedEvent : EventBase
    {
        public string message { get; set; }
        public string deviceSerialNumber { get; set; }
        public RealTimeMessageReceivedEvent(string deviceSerialNumber, string message)
        {
            this.message = message;
            this.deviceSerialNumber = deviceSerialNumber;
            EventType = EventType.RealTimeMessageReceived;
        }
    }
}
