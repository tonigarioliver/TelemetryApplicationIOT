using AutoMapper;
using IOTHistoricalDataService.Entity;
using IOTHistoricalDataService.Models.DTO;

namespace IOTHistoricalDataService.Models.Profiles
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
