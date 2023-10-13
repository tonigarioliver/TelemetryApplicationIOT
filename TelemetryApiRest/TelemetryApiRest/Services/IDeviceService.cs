using TelemetryApiRest.Models;
using TelemetryApiRest.Models.DTO;

namespace TelemetryApiRest.Services
{
    public interface IDeviceService
    {
        Task<DeviceServiceResponse> RegisterNewDevice(RegisterNewDeviceDTO newDeviceDTO);
        Task<DeviceServiceResponse> UpdateDevice(DeviceModel updateDevice, string serialNumber);
        Task<List<DeviceModel>> GetAllDevices();
    }
}
