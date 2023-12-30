namespace TelemetryApiRest.EventProcessing
{
    public class AddDevice : EventBase
    {
        public string deviceSerialNumber { get; set; }
        public AddDevice(string deviceSerialNumber)
        {
            this.deviceSerialNumber = deviceSerialNumber;
            EventType = EventType.AddDevice;
        }
    }
}

