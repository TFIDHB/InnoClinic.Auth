using InnoClinic.Appointments.API.Domain.Entities;
using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Appointments.API.Application.Interfaces
{
    public interface IResultRepository : IRepository<Result, Guid>
    {
        Task<Result?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken ct = default);

        Task<HashSet<Guid>> GetExistingAppointmentIdsAsync(IEnumerable<Guid> appointmentIds, CancellationToken ct = default);
    }
}
