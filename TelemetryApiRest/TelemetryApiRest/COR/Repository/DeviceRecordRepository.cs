using TelemetryApiRest.COR.IRepository;
using TelemetryApiRest.Data;
using TelemetryApiRest.Entity;

namespace TelemetryApiRest.COR.Repository
{
    public class DeviceRecordRepository : GenericRepository<DeviceRecord>, IDeviceRecordsRepository
    {
        public DeviceRecordRepository(TelemetryApiDbContext dbContext, 
            ILogger<IGenericRepository<DeviceRecord>> logger) : base(dbContext, logger)
        {
        }
    }
}
