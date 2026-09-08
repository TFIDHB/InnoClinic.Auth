using InnoClinic.Profiles.API.Domain.Entities;
using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Profiles.API.Application.Interfaces
{
    public interface IReceptionistProfilesRepository : IRepository<ReceptionistProfile, Guid>
    {
        Task<ReceptionistProfile?> GetByAccountIdAsync(Guid id, CancellationToken ct = default);
    }
}
