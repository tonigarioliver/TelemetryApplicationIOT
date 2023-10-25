using Microsoft.AspNetCore.Http;
using TelemetryApiRest.Models;
using TelemetryApiRest.Models.DTO;

namespace TelemetryApiRestUnitTest.MockData
{
    public class DeviceMockData
    {
        public static List<DeviceModel> GetSampleDevicesModel()
        {
            // Create a list of sample devices
            var devices = new List<DeviceModel>
            {
                new DeviceModel
                {
                    SerialNumber = "Device1Serial",
                    Vendor = "Vendor1",
                    Name = "Device 1",
                    isEnabled = true
                },
                new DeviceModel
                {
                    SerialNumber = "Device2Serial",
                    Vendor = "Vendor2",
                    Name = "Device 2",
                    isEnabled = true,
                },
                // Add more devices as needed
            };

            return devices;
        }
        public static DeviceServiceResponse GetCreatedResponse()
        {
            return new DeviceServiceResponse
            {
                StatusCode = StatusCodes.Status201Created,
                device = GetSampleDevicesModel()[0]
            };
        }
        public static DeviceServiceResponse GetUpdateResponse()
        {
            return new DeviceServiceResponse
            {
                StatusCode = StatusCodes.Status204NoContent,
                device = GetSampleDevicesModel()[0]
            };
        }
        public static RegisterNewDeviceDTO GetNewDevice()=>
            new RegisterNewDeviceDTO
            {
                SerialNumber = "12345",
                Vendor = "VendorName",
                Name = "DeviceName",
            };
     

    }
}