using InnoClinic.Appointments.API.Application.DTOs;

namespace InnoClinic.Appointments.API.Application.Interfaces
{
    public interface IResultService
    {
        Task<ResultDto> CreateAsync(
            Guid appointmentId,
            CreateResultRequestDto dto,
            Guid doctorId,
            CancellationToken ct = default);

        Task<ResultDto> UpdateAsync(
            Guid appointmentId,
            UpdateResultRequestDto dto,
            Guid doctorId,
            CancellationToken ct = default);

        Task<ResultDto> GetByAppointmentIdAsync(
            Guid appointmentId,
            Guid? doctorId,
            Guid? patientId,
            CancellationToken ct = default);

        Task<byte[]> GetOrGenerateResultFileAsync(
            Guid appointmentId,
            Guid patientId,
            CancellationToken ct = default);
    }
}
