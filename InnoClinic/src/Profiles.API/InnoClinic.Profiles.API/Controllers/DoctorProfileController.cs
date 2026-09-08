using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Profiles.API.Domain.Enums;
using InnoClinic.Shared.Constants;
using InnoClinic.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Profiles.API.Controllers
{
    [Authorize(Roles = Roles.AllRoles)]
    [ApiController]
    [Route("api/v1/doctors")]
    public class DoctorProfileController(
        IDoctorProfileService doctorProfilesService) : ControllerBase
    {
        [HttpGet("me")]
        [Authorize(Roles = Roles.Doctor)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorProfileDto>> GetMyProfile(CancellationToken ct = default)
        {
            var accountId = User.GetUserId();
            var result = await doctorProfilesService.GetByAccountIdAsync(accountId, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.AllRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorProfileDto>> GetDoctor(Guid id, CancellationToken ct = default)
        {
            var result = await doctorProfilesService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpPost("batch")]
        [Authorize(Roles = Roles.AllRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DoctorProfileDto>>> GetDoctorsByIds(
            [FromBody] IEnumerable<Guid> ids,
            CancellationToken ct = default)
        {
            var result = await doctorProfilesService.GetByIdsAsync(ids, ct);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = Roles.AllRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DoctorProfileDto>>> GetAllDoctors(
            [FromQuery] Guid? specializationId,
            [FromQuery] Guid? officeId,
            [FromQuery] string? search,
            CancellationToken ct = default)
        {
            DoctorStatus? status = User.IsInRole(Roles.Patient) ? DoctorStatus.AtWork : null;
            var result = await doctorProfilesService.GetFilteredDoctorsAsync(specializationId, officeId, search, status, ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorProfileDto>> CreateDoctor([FromBody] CreateDoctorProfileRequestDto dto, CancellationToken ct = default)
        {
            var result = await doctorProfilesService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetDoctor), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Receptionist + "," + Roles.Doctor)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorProfileDto>> UpdateDoctor(
            Guid id,
            [FromBody] UpdateDoctorProfileRequestDto dto,
            CancellationToken ct = default)
        {
            Guid? accountOwnerId = User.IsInRole(Roles.Doctor) ? User.GetUserId() : null;
            var result = await doctorProfilesService.UpdateAsync(id, dto, accountOwnerId, ct);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDoctor(Guid id, CancellationToken ct = default)
        {
            await doctorProfilesService.DeleteAsync(id, ct);
            return Ok();
        }
    }
}