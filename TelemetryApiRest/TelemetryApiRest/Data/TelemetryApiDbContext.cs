using Microsoft.EntityFrameworkCore;
using TelemetryApiRest.Entity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TelemetryApiRest.Data
{
    public class TelemetryApiDbContext:DbContext
    {
        public TelemetryApiDbContext(DbContextOptions<TelemetryApiDbContext>options)
            :base(options)
        {
            
        }
        DbSet<Device>devices { get; set; }
        DbSet<DeviceRecord>deviceRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>()
             .HasMany(r => r.DeviceRecords)
             .WithOne(d => d.device)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();

            base.OnModelCreating(modelBuilder);
        }

    }
}
