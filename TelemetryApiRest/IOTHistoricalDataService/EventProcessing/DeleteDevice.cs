namespace IOTHistoricalDataService.EventProcessing
{
    public class DeleteDevice:EventBase
    {
        public string deviceSerialNumber { get; set; }
        public DeleteDevice(string deviceSerialNumber)
        {
            this.deviceSerialNumber = deviceSerialNumber;
            EventType = EventType.DeleteDevice;
        }
    }
}
