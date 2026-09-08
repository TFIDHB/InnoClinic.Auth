using InnoClinic.Services.API.Domain.Entities;
using InnoClinic.Shared.Interfaces;

namespace InnoClinic.Services.API.Application.Interfaces
{
    public interface IServicesRepository : IRepository<Service, Guid>
    {
        Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);

        Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct = default);

        Task<int> GetTimeSlotSizeAsync(Guid serviceId, CancellationToken ct = default);
    }
}
