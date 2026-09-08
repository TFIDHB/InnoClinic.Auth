using InnoClinic.Services.API.Application.DTOs;
using InnoClinic.Services.API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Services.API.Controllers
{
    [Route("api/v1/specializations")]
    [ApiController]
    public class SpecializationsController(ISpecializationsService specializationsService) : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SpecializationDto>> GetSpecialization(Guid id, CancellationToken ct = default)
        {
            var result = await specializationsService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SpecializationDto>>> GetAllSpecializations(CancellationToken ct = default)
        {
            var result = await specializationsService.GetAllAsync(ct);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SpecializationDto>> CreateSpecialization(
            [FromBody] CreateSpecializationRequestDto dto,
            CancellationToken ct = default)
        {
            var result = await specializationsService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetSpecialization), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SpecializationDto>> UpdateSpecialization(
            Guid id,
            [FromBody] UpdateSpecializationRequestDto dto,
            CancellationToken ct = default)
        {
            var result = await specializationsService.UpdateAsync(id, dto, ct);
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SpecializationDto>> UpdateSpecializationStatus(
            Guid id,
            [FromBody] UpdateSpecializationStatusRequestDto dto,
            CancellationToken ct = default)
        {
            var result = await specializationsService.UpdateStatusAsync(id, dto, ct);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteSpecialization(Guid id, CancellationToken ct = default)
        {
            await specializationsService.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
