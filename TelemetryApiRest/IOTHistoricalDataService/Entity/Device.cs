using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOTHistoricalDataService.Entity
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("SerialNumber")]
        [Required]
        [StringLength(250)]
        [Unicode(false)]
        public string SerialNumber { get; set; } = null!;

        [Column("Vendor")]
        [StringLength(100)]
        [Unicode(false)]
        public string Vendor { get; set; } = null!;

        [Column("Name")]
        [StringLength(20)]
        [Unicode(false)]
        public string? Name { get; set; }

        public bool isEnabled { get; set; } = false;

        [Column("DeviceRecords")]
        public List<DeviceRecord> DeviceRecords{ get; set; }=new List<DeviceRecord>();
    }
}
