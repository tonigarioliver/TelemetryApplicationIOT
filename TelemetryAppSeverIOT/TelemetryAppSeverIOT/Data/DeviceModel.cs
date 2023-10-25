using System.Text.Json.Serialization;

namespace TelemetryAppSeverIOT.Data
{
    public class DeviceModel
    {
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; } = null!;

        [JsonPropertyName("vendor")]
        public string Vendor { get; set; } = null!;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("isEnabled")]
        public bool isEnabled { get; set; } = false;

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
