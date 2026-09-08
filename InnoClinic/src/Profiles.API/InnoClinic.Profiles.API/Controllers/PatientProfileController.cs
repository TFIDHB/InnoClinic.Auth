using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Shared.Constants;
using InnoClinic.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Profiles.API.Controllers
{
    [Authorize(Roles = Roles.AllRoles)]
    [ApiController]
    [Route("api/v1/patients")]
    public class PatientProfileController(
        IPatientProfileService patientProfilesService) : ControllerBase
    {
        [HttpGet("me")]
        [Authorize(Roles = Roles.Patient)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientProfileDto>> GetMyProfile(CancellationToken ct = default)
        {
            var accountId = User.GetUserId();
            var result = await patientProfilesService.GetByAccountIdAsync(accountId, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.AllRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientProfileDto>> GetPatient(Guid id, CancellationToken ct = default)
        {
            var result = await patientProfilesService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpPost("batch")]
        [Authorize(Roles = Roles.AllRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PatientProfileDto>>> GetPatientsByIds(
            [FromBody] IEnumerable<Guid> ids,
            CancellationToken ct = default)
        {
            var result = await patientProfilesService.GetByIdsAsync(ids, ct);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = Roles.Doctor + "," + Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PatientProfileDto>>> GetAllPatients(CancellationToken ct = default)
        {
            var result = await patientProfilesService.GetAllAsync(ct);
            return Ok(result);
        }

        [HttpGet("search")]
        [Authorize(Roles = Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PatientProfileDto>>> GetFilteredPatients(string? search, CancellationToken ct = default)
        {
            var result = await patientProfilesService.GetFilteredPatientsAsync(search, ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientProfileDto>> CreatePatient([FromBody] CreatePatientProfileRequestDto dto, CancellationToken ct = default)
        {
            var result = await patientProfilesService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetPatient), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Patient + "," + Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientProfileDto>> UpdatePatient(Guid id, [FromBody] UpdatePatientProfileRequestDto dto, CancellationToken ct = default)
        {
            Guid? accountOwnerId = User.IsInRole(Roles.Patient) ? User.GetUserId() : null;
            var result = await patientProfilesService.UpdateAsync(id, dto, accountOwnerId, ct);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePatient(Guid id, CancellationToken ct = default)
        {
            await patientProfilesService.DeleteAsync(id, ct);
            return Ok();
        }

        [HttpPost("create-my-profile")]
        [Authorize(Roles = Roles.Patient)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientProfileDto>> CreateMyProfile([FromBody] CreateMyPatientProfileRequestDto dto, CancellationToken ct = default)
        {
            var accountId = User.GetUserId();
            var result = await patientProfilesService.CreateOrMatchProfileAsync(accountId, dto, ct);
            return Ok(result);
        }

        [HttpPost("{id}/link-to-account")]
        [Authorize(Roles = Roles.Patient)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientProfileDto>> LinkProfileToAccount(Guid id, [FromBody] CreatePatientProfileRequestDto dto, CancellationToken ct = default)
        {
            var accountId = User.GetUserId();
            var result = await patientProfilesService.LinkProfileToAccountAsync(id, accountId, dto, ct);
            return Ok(result);
        }
    }
}