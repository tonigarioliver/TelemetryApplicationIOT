using TelemetryApiRest.COR.IRepository;

namespace TelemetryApiRest.COR
{
    public interface IUnitOfWork:IDisposable
    {
        IDeviceRecordsRepository DeviceRecordsRepository { get; }
        IDeviceRepository DeviceRepository { get; }
        Task CompleteAsync();
    }
}
