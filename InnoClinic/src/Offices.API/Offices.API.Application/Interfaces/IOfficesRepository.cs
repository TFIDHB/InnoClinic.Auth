using InnoClinic.Offices.API.Domain.Entities;

namespace InnoClinic.Offices.API.Application.Interfaces
{
    public interface IOfficesRepository
    {
        Task<Office?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<Office>> GetAllAsync(CancellationToken ct = default);

        Task CreateAsync(Office office, CancellationToken ct = default);

        Task UpdateAsync(Office office, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
