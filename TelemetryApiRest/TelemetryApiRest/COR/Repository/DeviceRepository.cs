using TelemetryApiRest.COR.IRepository;
using TelemetryApiRest.Data;
using TelemetryApiRest.Entity;

namespace TelemetryApiRest.COR.Repository
{
    public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(TelemetryApiDbContext dbContext,
            ILogger<IGenericRepository<Device>> logger) 
            : base(dbContext, logger)
        {
        }
    }
}
