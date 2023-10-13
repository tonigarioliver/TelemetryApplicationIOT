using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TelemetryApiRest.Entity;

namespace TelemetryApiRest.Models
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
