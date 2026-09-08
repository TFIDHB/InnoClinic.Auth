using InnoClinic.Appointments.API.Application.DTOs;
using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Shared.Constants;
using InnoClinic.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Appointments.API.Controllers
{
    [Authorize(Roles = Roles.AllRoles)]
    [ApiController]
    [Route("api/v1/appointments/{appointmentId}/result")]
    public class ResultController(IResultService resultService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = Roles.Doctor)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResultDto>> CreateResult(
            Guid appointmentId,
            [FromBody] CreateResultRequestDto dto,
            CancellationToken ct = default)
        {
            var doctorId = User.GetUserId();
            var result = await resultService.CreateAsync(appointmentId, dto, doctorId, ct);
            return CreatedAtAction(nameof(GetResult), new { appointmentId }, result);
        }

        [HttpPut]
        [Authorize(Roles = Roles.Doctor)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResultDto>> UpdateResult(
            Guid appointmentId,
            [FromBody] UpdateResultRequestDto dto,
            CancellationToken ct = default)
        {
            var doctorId = User.GetUserId();
            var result = await resultService.UpdateAsync(appointmentId, dto, doctorId, ct);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = Roles.AllRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResultDto>> GetResult(Guid appointmentId, CancellationToken ct = default)
        {
            Guid? doctorId = User.IsInRole(Roles.Doctor) ? User.GetUserId() : null;
            Guid? patientId = User.IsInRole(Roles.Patient) ? User.GetUserId() : null;
            var result = await resultService.GetByAppointmentIdAsync(appointmentId, doctorId, patientId, ct);
            return Ok(result);
        }

        [HttpGet("download")]
        [Authorize(Roles = Roles.Patient)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DownloadResult(Guid appointmentId, CancellationToken ct = default)
        {
            var patientId = User.GetUserId();
            var fileBytes = await resultService.GetOrGenerateResultFileAsync(appointmentId, patientId, ct);
            return File(fileBytes, "application/pdf", "appointment-result.pdf");
        }
    }
}
