using InnoClinic.Services.API.Application.DTOs;

namespace InnoClinic.Services.API.Application.Interfaces
{
    public interface IAppointmentsClient
    {
        Task<IEnumerable<AppointmentSlotDto>> GetAppointmentsAsync(
            DateOnly date,
            Guid? doctorId,
            CancellationToken ct = default);

        Task<IEnumerable<AppointmentSlotDto>> GetAppointmentsRangeAsync(
            DateOnly startDate,
            DateOnly endDate,
            Guid? doctorId,
            CancellationToken ct = default);
    }
}
