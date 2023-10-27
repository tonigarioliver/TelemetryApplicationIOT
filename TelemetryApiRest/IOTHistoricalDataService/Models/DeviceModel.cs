
namespace IOTHistoricalDataService.Models
{
    public class DeviceModel
    {
        public string SerialNumber { get; set; } = null!;
        public string Vendor { get; set; } = null!;
        public string? Name { get; set; }
        public bool isEnabled { get; set; } = false;
        public string Status { get; set; }
    }
}
