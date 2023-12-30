namespace TelemetryApiRest.EventProcessing
{
    public enum EventType
    {
        RealTimeMessageReceived,
        MqttBackgroundMessageReceived,
        DeleteDevice,
        AddDevice
    }
}
