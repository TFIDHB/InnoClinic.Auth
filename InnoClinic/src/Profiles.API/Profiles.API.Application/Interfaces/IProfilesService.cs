using InnoClinic.Profiles.API.Application.DTOs;

namespace InnoClinic.Profiles.API.Application.Interfaces
{
    public interface IProfilesService<TDto, TCreateDto, TUpdateDto>
    {
        Task<TDto> CreateAsync(TCreateDto dto, CancellationToken ct = default);

        Task<TDto> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken ct = default);

        Task<TDto> UpdateAsync(Guid id, TUpdateDto dto, Guid? accountOwnerId = null, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task<AccountProfileInfoDto?> GetProfileInfoByAccountIdAsync(Guid id, CancellationToken ct = default);
    }
}
