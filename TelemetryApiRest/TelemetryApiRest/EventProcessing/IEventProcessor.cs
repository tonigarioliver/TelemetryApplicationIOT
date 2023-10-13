namespace TelemetryApiRest.EventProcessing
{
    // Define an interface for event processors
    public interface IEventProcessor
    {
        Task ProcessEventAsync(EventBase @event);
    }
}