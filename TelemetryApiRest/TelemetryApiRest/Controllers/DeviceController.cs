using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TelemetryApiRest.Entity;
using TelemetryApiRest.EventProcessing;
using TelemetryApiRest.Models;
using TelemetryApiRest.Models.DTO;
using TelemetryApiRest.Services;

namespace TelemetryApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private IDeviceService deviceService;
        private readonly IWebHostEnvironment environment;
        private readonly IEventProcessor eventProcessor;
        private IServiceProvider serviceProvider;
        public DeviceController(IDeviceService deviceService, IWebHostEnvironment environment, IEventProcessor eventProcessor)
        {
            this.deviceService = deviceService;
            this.environment = environment;
            this.eventProcessor = eventProcessor;
        }
        [HttpGet]
        [Route("GetAllDevices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var customers = await deviceService.GetAllDevices();
            return Ok(customers);
        }
        [HttpPost]
        [Route("RegisterNewDevice")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterNewDevice([FromBody] RegisterNewDeviceDTO newDevice)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DeviceServiceResponse response = await deviceService.RegisterNewDevice(newDevice);

            switch (response.StatusCode)
            {
                case StatusCodes.Status201Created:
                    await eventProcessor.ProcessEventAsync(new AddDevice(newDevice.SerialNumber));
                    return StatusCode(StatusCodes.Status201Created);
                case StatusCodes.Status500InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                default:
                    return BadRequest(response);
            }
        }

        [HttpPut]
        [Route("UpdateDevice/{serialNumber}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDevice([FromRoute] string serialNumber, [FromBody] DeviceModel update)
        {
            var response = await deviceService.UpdateDevice(update, serialNumber);
            IActionResult result;
            switch (response.StatusCode)
            {
                case StatusCodes.Status204NoContent:
                    result = NoContent();
                    break;
                case StatusCodes.Status500InternalServerError:
                    result = StatusCode(StatusCodes.Status500InternalServerError, response);
                    break;
                case StatusCodes.Status404NotFound:
                    result = NotFound(response.device);
                    break;
                default:
                    result = BadRequest(response);
                    break;
            }
            return result;
        }
        [HttpDelete]
        [Route("DeleteDevice/{serialNumber}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteDevice([FromRoute] string serialNumber)
        {
            var response = await deviceService.RemoveAsync(serialNumber);
            IActionResult result;
            switch (response.StatusCode)
            {
                case StatusCodes.Status204NoContent:
                    await eventProcessor.ProcessEventAsync(new DeleteDevice(serialNumber));
                    result = NoContent();
                    break;
                case StatusCodes.Status500InternalServerError:
                    result = StatusCode(StatusCodes.Status500InternalServerError, response);
                    break;
                case StatusCodes.Status404NotFound:
                    result = NotFound(response.Errormessage);
                    break;
                default:
                    result = BadRequest(response);
                    break;
            }
            return result;
        }
    }
}
