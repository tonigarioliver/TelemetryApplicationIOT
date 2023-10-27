using IOTHistoricalDataService.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTHistoricalDataService.Data
{
    public class appDbContext : DbContext
    {
        public appDbContext(DbContextOptions<appDbContext> options)
        : base(options)
        {

        }
        DbSet<Device> devices { get; set; }
        DbSet<DeviceRecord> deviceRecords { get; set; }

    }
}
