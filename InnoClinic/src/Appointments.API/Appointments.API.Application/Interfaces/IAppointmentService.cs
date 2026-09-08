using InnoClinic.Appointments.API.Application.DTOs;

namespace InnoClinic.Appointments.API.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto> CreateAsync(CreateAppointmentRequestDto dto, bool isPatient, CancellationToken ct = default);

        Task<AppointmentResponseDto> GetByIdAsync(
            Guid appointmentId,
            Guid? patientId,
            Guid? doctorId,
            CancellationToken ct = default);

        Task<IEnumerable<AppointmentSlotDto>> GetSlotsByDateAndDoctorAsync(
            DateOnly date,
            Guid? doctorId,
            CancellationToken ct = default);

        Task<IEnumerable<AppointmentSlotDto>> GetSlotsByDateRangeAndDoctorAsync(
            DateOnly startDate,
            DateOnly endDate,
            Guid? doctorId,
            CancellationToken ct = default);

        Task<IEnumerable<ScheduleDto>> GetDoctorAppointmentScheduleAsync(
            Guid doctorId,
            DateOnly date,
            CancellationToken ct = default);

        Task<IEnumerable<ScheduleDto>> GetMyScheduleAsync(DateOnly date, CancellationToken ct = default);

        Task<IEnumerable<AppointmentListItemDto>> GetFilteredAppointmentsAsync(
            DateOnly? date,
            Guid? officeId,
            bool? isApproved,
            string? doctorFullName,
            string? serviceName,
            CancellationToken ct = default);

        Task ApproveAsync(Guid id, CancellationToken ct = default);

        Task CancelAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<AppointmentHistoryItemDto>> GetPatientHistoryAsync(Guid patientId, CancellationToken ct = default);

        Task<IEnumerable<AppointmentHistoryItemDto>> GetMyHistoryAsync(CancellationToken ct = default);

        Task<AppointmentResponseDto> RescheduleAsync(
            Guid appointmentId,
            RescheduleAppointmentRequestDto dto,
            Guid? patientId,
            CancellationToken ct = default);
    }
}
