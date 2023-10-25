using System.ComponentModel.DataAnnotations;

namespace TelemetryApiRest.Models.DTO
{
    public class RegisterNewDeviceDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Serial Number is required")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Vendor is required")]
        public string Vendor { get; set; }
    }
}
