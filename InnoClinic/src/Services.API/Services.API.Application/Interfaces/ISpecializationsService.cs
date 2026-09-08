using InnoClinic.Services.API.Application.DTOs;

namespace InnoClinic.Services.API.Application.Interfaces
{
    public interface ISpecializationsService
    {
        Task<SpecializationDto> CreateAsync(CreateSpecializationRequestDto dto, CancellationToken ct = default);

        Task<SpecializationDto> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<SpecializationDto>> GetAllAsync(CancellationToken ct = default);

        Task<SpecializationDto> UpdateAsync(Guid id, UpdateSpecializationRequestDto dto, CancellationToken ct = default);

        Task<SpecializationDto> UpdateStatusAsync(Guid id, UpdateSpecializationStatusRequestDto dto, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
