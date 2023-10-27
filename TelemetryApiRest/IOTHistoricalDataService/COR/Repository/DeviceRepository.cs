

using IOTHistoricalDataService.COR.IRepository;
using IOTHistoricalDataService.Data;
using IOTHistoricalDataService.Entity;
using Microsoft.Extensions.Logging;

namespace IOTHistoricalDataService.COR.Repository
{
    public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(appDbContext dbContext,
            ILogger<IGenericRepository<Device>> logger) 
            : base(dbContext, logger)
        {
        }
    }
}
