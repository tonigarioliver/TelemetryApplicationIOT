
using IOTHistoricalDataService.COR.IRepository;

namespace IOTHistoricalDataService.COR
{
    public interface IUnitOfWork:IDisposable
    {
        IDeviceRecordsRepository DeviceRecordsRepository { get; }
        IDeviceRepository DeviceRepository { get; }
        Task CompleteAsync();
    }
}
