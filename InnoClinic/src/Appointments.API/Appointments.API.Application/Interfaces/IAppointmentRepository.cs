using InnoClinic.Appointments.API.Domain.Entities;
using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Appointments.API.Application.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment, Guid>
    {
        Task<IEnumerable<Appointment>> GetByDateAndDoctorAsync(
            DateOnly date,
            Guid? doctorId,
            CancellationToken ct = default);

        Task<IEnumerable<Appointment>> GetByDateRangeAndDoctorAsync(
            DateOnly startDate,
            DateOnly endDate,
            Guid? doctorId,
            CancellationToken ct = default);

        Task<IEnumerable<Appointment>> GetFilteredAsync(
                DateOnly? date,
                Guid? officeId,
                bool? isApproved,
                CancellationToken ct = default);

        Task<IEnumerable<Appointment>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    }
}
