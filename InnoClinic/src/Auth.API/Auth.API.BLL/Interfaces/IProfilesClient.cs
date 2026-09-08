using InnoClinic.Auth.API.BLL.DTOs;

namespace InnoClinic.Auth.API.BLL.Interfaces
{
    public interface IProfilesClient
    {
        Task<AccountProfileInfoDto?> GetProfileInfoByAccountIdAsync(Guid id, CancellationToken ct = default);
    }
}
