using AutoMapper;
using TelemetryApiRest.Entity;
using TelemetryApiRest.Models.DTO;

namespace TelemetryApiRest.Models.Profiles
{
    public class DeviceProfile:Profile
    {
        public DeviceProfile()
        {
            CreateMap<Device, DeviceModel>().ForMember(item => item.Status, opt => opt.MapFrom(
             item => (item.isEnabled != null) ? "Active" : "In active")).ReverseMap();
            CreateMap<RegisterNewDeviceDTO, DeviceModel>().ReverseMap();
        }
    }
}
