using InnoClinic.Offices.API.Application.DTOs;

namespace InnoClinic.Offices.API.Application.Interfaces
{
    public interface IOfficesService
    {
        Task<OfficeDto> CreateAsync(CreateOfficeRequestDto dto, CancellationToken ct = default);

        Task<OfficeDto> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<OfficeDto>> GetAllAsync(CancellationToken ct = default);

        Task<OfficeDto> UpdateAsync(Guid id, UpdateOfficeRequestDto dto, CancellationToken ct = default);

        Task<OfficeDto> UpdateStatusAsync(Guid id, UpdateOfficeStatusRequestDto dto, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
