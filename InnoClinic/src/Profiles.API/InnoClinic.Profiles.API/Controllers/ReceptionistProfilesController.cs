using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Profiles.API.Controllers
{
    [Authorize(Roles = Roles.Receptionist)]
    [ApiController]
    [Route("api/v1/receptionists")]
    public class ReceptionistProfilesController(
        IProfilesService<ReceptionistProfileDto, CreateReceptionistProfileRequestDto, UpdateReceptionistProfileRequestDto> receptionistProfilesService) : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReceptionistProfileDto>> GetReceptionist(Guid id, CancellationToken ct = default)
        {
            var result = await receptionistProfilesService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ReceptionistProfileDto>>> GetAllReceptionists(CancellationToken ct = default)
        {
            var result = await receptionistProfilesService.GetAllAsync(ct);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReceptionistProfileDto>> CreateReceptionist([FromBody] CreateReceptionistProfileRequestDto dto, CancellationToken ct = default)
        {
            var result = await receptionistProfilesService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetReceptionist), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReceptionistProfileDto>> UpdateReceptionist(
            Guid id,
            [FromBody] UpdateReceptionistProfileRequestDto dto,
            CancellationToken ct = default)
        {
            var result = await receptionistProfilesService.UpdateAsync(id, dto, ct: ct);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteReceptionist(Guid id, CancellationToken ct = default)
        {
            await receptionistProfilesService.DeleteAsync(id, ct);
            return Ok();
        }
    }
}