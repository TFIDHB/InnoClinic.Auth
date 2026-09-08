using InnoClinic.Profiles.API.Application.DTOs;

namespace InnoClinic.Profiles.API.Application.Interfaces
{
    public interface IAccountSearchService
    {
        Task<AccountProfileInfoDto?> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    }
}
