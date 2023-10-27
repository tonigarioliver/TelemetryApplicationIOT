namespace IOTHistoricalDataService.EventProcessing
{
    public class MqttBackgroundMessageReceivedEvent : EventBase
    {
        public string message { get; set; }
        public string deviceSerialNumber { get; set; }
        public MqttBackgroundMessageReceivedEvent(string deviceSerialNumber, string message)
        {
            this.message = message;
            this.deviceSerialNumber = deviceSerialNumber;
            EventType = EventType.MqttBackgroundMessageReceived;
        }
    }
}
