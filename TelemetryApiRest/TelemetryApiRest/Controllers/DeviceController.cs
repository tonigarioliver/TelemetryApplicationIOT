using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelemetryApiRest.Entity;
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
        public DeviceController(IDeviceService deviceService, IWebHostEnvironment environment)
        {
            this.deviceService = deviceService;
            this.environment = environment;
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
        public async Task<ActionResult<Device>> RegisterNewDevice([FromBody] RegisterNewDeviceDTO newDevice)
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
                    return Ok(response);
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
        public async Task<IActionResult> UpdateCustomer([FromRoute] string serialNumber, [FromBody] DeviceModel update)
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
    }
}
