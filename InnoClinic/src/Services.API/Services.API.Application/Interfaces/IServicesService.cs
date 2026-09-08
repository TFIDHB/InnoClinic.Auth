using InnoClinic.Services.API.Application.DTOs;

namespace InnoClinic.Services.API.Application.Interfaces
{
    public interface IServicesService
    {
        Task<ServiceDto> CreateAsync(CreateServiceRequestDto dto, CancellationToken ct = default);

        Task<ServiceDto> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<ServiceDto>> GetAllAsync(CancellationToken ct = default);

        Task<ServiceDto> UpdateAsync(Guid id, UpdateServiceRequestDto dto, CancellationToken ct = default);

        Task<ServiceDto> UpdateStatusAsync(Guid id, UpdateServiceStatusRequestDto dto, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<TimeOnly>> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto dto, CancellationToken ct = default);

        Task<IEnumerable<DateOnly>> GetAvailableDatesAsync(GetAvailableDatesRequestDto dto, CancellationToken ct = default);

        Task<int> GetTimeSlotSizeAsync(Guid serviceId, CancellationToken ct = default);

        Task<IEnumerable<ServiceDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    }
}
