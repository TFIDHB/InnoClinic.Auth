using InnoClinic.Profiles.API.Application.DTOs;

namespace InnoClinic.Profiles.API.Application.Interfaces
{
    public interface IAuthClient
    {
        Task<CreateStaffAccountResponseDto> CreateStaffAccountAsync(string email, CancellationToken ct = default);

        Task<UserAccountInfoDto?> GetUserAccountInfoAsync(Guid userId, CancellationToken ct = default);

        Task<UserAccountInfoDto?> GetAccountInfoAsStaffAsync(Guid userId, CancellationToken ct = default);

        Task UpdateUserAccountInfoAsync(Guid userId, UpdateUserAccountInfoDto dto, CancellationToken ct = default);
    }
}
