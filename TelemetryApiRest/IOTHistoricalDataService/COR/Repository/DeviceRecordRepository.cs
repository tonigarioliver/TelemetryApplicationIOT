

using IOTHistoricalDataService.COR.IRepository;
using IOTHistoricalDataService.Data;
using IOTHistoricalDataService.Entity;
using Microsoft.Extensions.Logging;

namespace IOTHistoricalDataService.COR.Repository
{
    public class DeviceRecordRepository : GenericRepository<DeviceRecord>, IDeviceRecordsRepository
    {
        public DeviceRecordRepository(appDbContext dbContext, 
            ILogger<IGenericRepository<DeviceRecord>> logger) : base(dbContext, logger)
        {
        }
    }
}
