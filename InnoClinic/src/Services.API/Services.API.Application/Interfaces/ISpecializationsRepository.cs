using InnoClinic.Services.API.Domain.Entities;
using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Services.API.Application.Interfaces
{
    public interface ISpecializationsRepository : IRepository<Specialization, Guid>
    {
        Task<Specialization?> GetByIdWithServicesAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<Specialization>> GetAllWithServicesAsync(CancellationToken ct = default);
    }
}
