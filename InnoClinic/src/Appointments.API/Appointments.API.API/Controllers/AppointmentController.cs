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
    [Route("api/v1/appointments")]
    public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
    {
        [HttpGet("{id}")]
        [Authorize(Roles = Roles.AllRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentResponseDto>> GetAppointment(Guid id, CancellationToken ct = default)
        {
            Guid? patientId = User.IsInRole(Roles.Patient) ? User.GetUserId() : null;
            Guid? doctorId = User.IsInRole(Roles.Doctor) ? User.GetUserId() : null;
            var result = await appointmentService.GetByIdAsync(id, patientId, doctorId, ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Patient + "," + Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment(
            [FromBody] CreateAppointmentRequestDto dto,
            CancellationToken ct = default)
        {
            var isPatient = User.IsInRole(Roles.Patient);
            var result = await appointmentService.CreateAsync(dto, isPatient, ct);
            return CreatedAtAction(nameof(GetAppointment), new { id = result.Id }, result);
        }

        [HttpGet("slots")]
        [Authorize(Roles = Roles.Patient + "," + Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AppointmentSlotDto>>> GetAppointmentSlots(
            [FromQuery] DateOnly date,
            [FromQuery] Guid? doctorId,
            CancellationToken ct = default)
        {
            var result = await appointmentService.GetSlotsByDateAndDoctorAsync(date, doctorId, ct);
            return Ok(result);
        }

        [HttpGet("range")]
        [Authorize(Roles = Roles.Patient + "," + Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAppointmentsByRange(
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate,
            [FromQuery] Guid? doctorId,
            CancellationToken ct = default)
        {
            var result = await appointmentService.GetSlotsByDateRangeAndDoctorAsync(startDate, endDate, doctorId, ct);
            return Ok(result);
        }

        [HttpGet("doctors/{doctorId}/schedule")]
        [Authorize(Roles = Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetDoctorSchedule(
            Guid doctorId,
            [FromQuery] DateOnly? date,
            CancellationToken ct = default)
        {
            var requiredDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var result = await appointmentService.GetDoctorAppointmentScheduleAsync(doctorId, requiredDate, ct);
            return Ok(result);
        }

        [HttpGet("my-schedule")]
        [Authorize(Roles = Roles.Doctor)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetMySchedule([FromQuery] DateOnly? date, CancellationToken ct = default)
        {
            var requiredDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var result = await appointmentService.GetMyScheduleAsync(requiredDate, ct);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AppointmentListItemDto>>> GetFilteredAppointments(
            [FromQuery] DateOnly? date,
            [FromQuery] Guid? officeId,
            [FromQuery] bool? isApproved,
            [FromQuery] string? doctorFullName,
            [FromQuery] string? serviceName,
            CancellationToken ct = default)
        {
            var result = await appointmentService.GetFilteredAppointmentsAsync(date, officeId, isApproved, doctorFullName, serviceName, ct);
            return Ok(result);
        }

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApproveAppointment(Guid id, CancellationToken ct = default)
        {
            await appointmentService.ApproveAsync(id, ct);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CancelAppointment(Guid id, CancellationToken ct = default)
        {
            await appointmentService.CancelAsync(id, ct);
            return NoContent();
        }

        [HttpGet("patients/{patientId}/history")]
        [Authorize(Roles = Roles.Doctor + "," + Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AppointmentHistoryItemDto>>> GetPatientHistory(Guid patientId, CancellationToken ct = default)
        {
            var result = await appointmentService.GetPatientHistoryAsync(patientId, ct);
            return Ok(result);
        }

        [HttpGet("my-history")]
        [Authorize(Roles = Roles.Patient)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AppointmentHistoryItemDto>>> GetMyHistory(CancellationToken ct = default)
        {
            var result = await appointmentService.GetMyHistoryAsync(ct);
            return Ok(result);
        }

        [HttpPut("{id}/reschedule")]
        [Authorize(Roles = Roles.Patient + "," + Roles.Receptionist)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentResponseDto>> RescheduleAppointment(
            Guid id,
            [FromBody] RescheduleAppointmentRequestDto dto,
            CancellationToken ct = default)
        {
            Guid? patientId = User.IsInRole(Roles.Patient) ? User.GetUserId() : null;
            var result = await appointmentService.RescheduleAsync(id, dto, patientId, ct);
            return Ok(result);
        }
    }
}
