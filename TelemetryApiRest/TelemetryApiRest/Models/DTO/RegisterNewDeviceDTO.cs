namespace TelemetryApiRest.Models.DTO
{
    public class RegisterNewDeviceDTO
    {
        public string SerialNumber { get; set; } = null!;
        public string Vendor { get; set; } = null!;
        public string? Name { get; set; }
    }
}
