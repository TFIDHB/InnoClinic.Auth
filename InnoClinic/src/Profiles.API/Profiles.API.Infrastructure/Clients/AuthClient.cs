using System.Net.Http.Json;
using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Profiles.API.Infrastructure.Clients
{
    public class AuthClient(HttpClient httpClient): IAuthClient
    {
        public async Task<CreateStaffAccountResponseDto> CreateStaffAccountAsync(string email, CancellationToken ct = default)
        {
            var response = await httpClient.PostAsJsonAsync("/api/v1/auth/create-staff-account", new { Email = email }, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CreateStaffAccountResponseDto>(ct)
                ?? throw new ExternalServiceException("Auth.API");
        }

        public async Task<UserAccountInfoDto?> GetAccountInfoAsStaffAsync(Guid userId, CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/accounts/{userId}", ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<UserAccountInfoDto>(ct)
                ?? throw new ExternalServiceException("Auth.API");
        }

        public async Task<UserAccountInfoDto?> GetUserAccountInfoAsync(Guid userId, CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/accounts/{userId}", ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<UserAccountInfoDto>(ct)
                ?? throw new ExternalServiceException("Auth.API");
        }

        public async Task UpdateUserAccountInfoAsync(Guid userId, UpdateUserAccountInfoDto dto, CancellationToken ct = default)
        {
            var response = await httpClient.PutAsJsonAsync($"/api/v1/accounts/{userId}", dto, ct);
            response.EnsureSuccessStatusCode();
        }
    }
}
