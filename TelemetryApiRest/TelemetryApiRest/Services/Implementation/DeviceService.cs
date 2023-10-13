﻿using AutoMapper;
using TelemetryApiRest.COR;
using TelemetryApiRest.Data;
using TelemetryApiRest.Entity;
using TelemetryApiRest.Models;
using TelemetryApiRest.Models.DTO;

namespace TelemetryApiRest.Services.Implementation
{
    public class DeviceService : IDeviceService
    {
        public IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<IDeviceService> logger;

        public DeviceService(IUnitOfWork unitOfWork, IMapper mapper, 
            ILogger<IDeviceService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<DeviceServiceResponse> RegisterNewDevice(RegisterNewDeviceDTO newDeviceDTO)
        {
            if (newDeviceDTO == null)
            {
                return new DeviceServiceResponse()
                {
                    device = null,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errormessage = "Null reference"
                };
            }
            DeviceModel newDevice = mapper.Map<DeviceModel>(newDeviceDTO);
            if (await unitOfWork.DeviceRepository.GetAsync(u => u.SerialNumber.ToLower()==newDevice.SerialNumber.ToLower()) != null)
            {
                return new DeviceServiceResponse()
                {
                    device = newDevice,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errormessage = "DeviceAlreadyExists"
                };
            }
            await unitOfWork.DeviceRepository.CreateAsync(mapper.Map<Device>(newDevice));
            await unitOfWork.CompleteAsync();
            return new DeviceServiceResponse()
            {
                device = newDevice,
                StatusCode = StatusCodes.Status201Created
            };
        }

        public async Task<DeviceServiceResponse> UpdateDevice(DeviceModel updateDevice, string serialNumber)
        {
            DeviceServiceResponse response = new DeviceServiceResponse();
            try
            {
                // Buscar el cliente existente por nombre
                var existingDevice = await unitOfWork.DeviceRepository.GetAsync(device => device.SerialNumber == serialNumber);

                if (existingDevice != null)
                {
                    // Usar AutoMapper para mapear las propiedades del modelo actualizado al cliente existente
                    mapper.Map(updateDevice, existingDevice);
                    await unitOfWork.DeviceRepository.Update(existingDevice);
                    // Guardar los cambios en la base de datos
                    await unitOfWork.CompleteAsync();
                    response.StatusCode = StatusCodes.Status204NoContent;
                    response.device = updateDevice;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Errormessage = "Data not found";
                    response.device = updateDevice;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Errormessage = ex.Message;
            }
            return response;
        }

        public async Task<List<DeviceModel>> GetAllDevices()
        {
            var devices = await unitOfWork.DeviceRepository.GetAllAsync();
            return mapper.Map<List<DeviceModel>>(devices);
        }
    }
}
