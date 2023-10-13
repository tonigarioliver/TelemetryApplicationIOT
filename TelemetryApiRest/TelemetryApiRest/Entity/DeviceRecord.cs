using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TelemetryApiRest.Entity
{
    public class DeviceRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string lastRecord { get; set; }
        public Device device { get; set; }

    }
}
