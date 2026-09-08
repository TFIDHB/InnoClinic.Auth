using InnoClinic.Profiles.API.Domain.Entities;
using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Profiles.API.Application.Interfaces
{
    public interface IPatientProfilesRepository : IRepository<PatientProfile, Guid>
    {
        Task<PatientProfile?> GetByAccountIdAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<PatientProfile>> GetUnlinkedProfilesAsync(CancellationToken ct = default);

        Task<IEnumerable<PatientProfile>> GetFilteredAsync(string? search, CancellationToken ct = default);

        Task<IEnumerable<PatientProfile>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    }
}
