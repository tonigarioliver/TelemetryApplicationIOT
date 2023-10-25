using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Moq;
using TelemetryApiRest.Controllers;
using TelemetryApiRest.Models.DTO;
using TelemetryApiRest.Services;
using Xunit;
using TelemetryApiRest.Models;
using TelemetryApiRestUnitTest.MockData;
using TelemetryApiRest.Entity;

public class DeviceControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        // Arrange
        var deviceServiceMock = new Mock<IDeviceService>();
        var environmentMock = new Mock<IWebHostEnvironment>();

        // Create a list of sample devices
        var sampleDevices = DeviceMockData.GetSampleDevicesModel();

        deviceServiceMock.Setup(_ => _.GetAllDevices()).ReturnsAsync(sampleDevices);
        var controller = new DeviceController(deviceServiceMock.Object, environmentMock.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }


    [Fact]
    public async Task RegisterNewDevice_ValidModel_ReturnsCreatedResult()
    {
        // Arrange
        var deviceServiceMock = new Mock<IDeviceService>();
        var environmentMock = new Mock<IWebHostEnvironment>();
        var controller = new DeviceController(deviceServiceMock.Object, environmentMock.Object);
        var sampleDevices = DeviceMockData.GetSampleDevicesModel();
        var expectedResponse = DeviceMockData.GetCreatedResponse();
        var newDeviceDTO = DeviceMockData.GetNewDevice();
        deviceServiceMock
            .Setup(_ => _.RegisterNewDevice(newDeviceDTO))
            .ReturnsAsync(expectedResponse);

        // Act
        IActionResult response = await controller.RegisterNewDevice(newDeviceDTO);

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(response);
        Assert.Equal(expectedResponse.StatusCode, statusCodeResult.StatusCode);
    }

    // Add more test cases for RegisterNewDevice to cover other scenarios (e.g., ModelState.IsValid, errors).

    [Fact]
    public async Task UpdateDevice_ValidModel_ReturnsNoContent()
    {
        // Arrange
        var deviceServiceMock = new Mock<IDeviceService>();
        var environmentMock = new Mock<IWebHostEnvironment>();
        var controller = new DeviceController(deviceServiceMock.Object, environmentMock.Object);
        var serialNumber = "12345";
        var update = new DeviceModel
        {
            SerialNumber = "12345",
            Vendor = "UpdatedVendor",
            Name = "UpdatedName",
            isEnabled = true,
            Status = "Active",
        };
        deviceServiceMock
            .Setup(_ => _.UpdateDevice(update, serialNumber))
            .ReturnsAsync(DeviceMockData.GetUpdateResponse());


        // Act
        var result = await controller.UpdateDevice(serialNumber, update);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

}
