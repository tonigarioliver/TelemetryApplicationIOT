using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelemetryApiRest.COR;
using TelemetryApiRest.Entity;
using TelemetryApiRest.Models.DTO;
using TelemetryApiRest.Models;
using TelemetryApiRest.Services.Implementation;
using TelemetryApiRest.Services;
using System.Linq.Expressions;
using TelemetryApiRest.COR.IRepository;

namespace TelemetryApiRestUnitTest.Services
{
    public class DeviceServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<IDeviceService>> _loggerMock;

        public DeviceServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<IDeviceService>>();
        }

        [Fact]
        public async Task RegisterNewDevice_DeviceDoesNotExist_ReturnsCreatedResponse()
        {
            // Arrange
            var deviceRepositoryMock = new Mock<IDeviceRepository>();
            var deviceService = new DeviceService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );

            var newDeviceDTO = new RegisterNewDeviceDTO
            {
                SerialNumber = "12345",
                Vendor = "VendorName",
                Name = "DeviceName",
            };

            var newDeviceModel = new DeviceModel
            {
                SerialNumber = "12345",
                Vendor = "VendorName",
                Name = "DeviceName",
            };
            var createdDevice = new Device
            {
                Id=1,
                SerialNumber = "12345",
                Vendor = "VendorName",
                Name = "DeviceName",
            };

            _unitOfWorkMock.Setup(u => u.DeviceRepository).Returns(deviceRepositoryMock.Object);
            deviceRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Device, bool>>>(), It.IsAny<bool>())).ReturnsAsync((Device)null);
            _mapperMock.Setup(mapper => mapper.Map<DeviceModel>(newDeviceDTO)).Returns(newDeviceModel);
            deviceRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Device>())).ReturnsAsync(createdDevice); // Create a new Device object here
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var response = await deviceService.RegisterNewDevice(newDeviceDTO);

            // Assert
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal(newDeviceModel, response.device);
        }


        [Fact]
        public async Task UpdateDevice_ExistingDevice_ReturnsNoContentResponse()
        {
            // Arrange
            var deviceRepositoryMock = new Mock<IDeviceRepository>();

            var deviceService = new DeviceService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );

            var updateDevice = new DeviceModel
            {
                SerialNumber = "12345",
                Vendor = "VendorName",
                Name = "DeviceName",
            };

            var existingDevice = new Device
            {
                SerialNumber = "12345",
            };

            _unitOfWorkMock.Setup(u => u.DeviceRepository).Returns(deviceRepositoryMock.Object);
            deviceRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Device, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existingDevice);
            _unitOfWorkMock.Setup(u => u.DeviceRepository.Update(It.IsAny<Device>())).Callback(() => { });
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var response = await deviceService.UpdateDevice(updateDevice, "12345");

            // Assert
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            Assert.Equal(updateDevice, response.device);
        }
    }
}
