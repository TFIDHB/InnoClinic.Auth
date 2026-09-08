using InnoClinic.Auth.API.BLL.DTOs;
using System.Security.Claims;

namespace InnoClinic.Auth.API.BLL.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default);

        Task<AuthTokenDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);

        Task LogoutAsync(LogOutRequestDto dto, Guid userId, CancellationToken ct = default);

        Task<CreateStaffAccountResponseDto> CreateStaffAccountAsync(CreateStaffAccountRequestDto dto, CancellationToken ct = default);

        Task<UserAccountInfoDto> GetUserAccountInfo(Guid userId, ClaimsPrincipal currentUser, CancellationToken ct = default);

        Task UpdateUserAccountInfo(Guid userId, UpdateUserAccountInfoDto dto, ClaimsPrincipal currentUser, CancellationToken ct = default);
    }
}
