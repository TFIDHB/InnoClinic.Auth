using InnoClinic.Auth.API.BLL.DTOs;
using InnoClinic.Auth.API.BLL.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace InnoClinic.Auth.API.BLL.Clients
{
    public class ProfilesClient(HttpClient httpClient) : IProfilesClient
    {
        public async Task<AccountProfileInfoDto?> GetProfileInfoByAccountIdAsync(Guid id, CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/accounts/{id}/profile-info", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AccountProfileInfoDto>(cancellationToken: ct);
        }
    }
}
