namespace IOTHistoricalDataService.EventProcessing
{
    public class EventBase
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public EventType EventType { get; set; }
    }
}
