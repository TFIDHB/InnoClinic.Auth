using InnoClinic.Services.API.Application.DTOs;
using InnoClinic.Services.API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Services.API.Controllers
{
    [Route("api/v1/time-slots")]
    [ApiController]
    public class TimeSlotsController(IServicesService servicesService) : ControllerBase
    {
        [HttpGet("slots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TimeOnly>>> GetAvailableSlots(
            [FromQuery] GetAvailableSlotsRequestDto dto,
            CancellationToken ct = default)
        {
            var result = await servicesService.GetAvailableSlotsAsync(dto, ct);
            return Ok(result);
        }

        [HttpGet("dates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DateOnly>>> GetAvailableDates(
            [FromQuery] GetAvailableDatesRequestDto dto,
            CancellationToken ct = default)
        {
            var result = await servicesService.GetAvailableDatesAsync(dto, ct);
            return Ok(result);
        }
    }
}
