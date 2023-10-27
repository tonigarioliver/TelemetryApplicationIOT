

using IOTHistoricalDataService.Models;
using IOTHistoricalDataService.Models.DTO;

namespace IOTHistoricalDataService.Services
{
    public interface IDeviceService
    {
        Task<DeviceServiceResponse> RegisterNewDevice(RegisterNewDeviceDTO newDeviceDTO);
        Task<DeviceServiceResponse> UpdateDevice(DeviceModel updateDevice, string serialNumber);
        Task<List<DeviceModel>> GetAllDevices();
        Task<DeviceServiceResponse> RemoveAsync(string serialNumber);
    }
}
