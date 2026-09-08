using System.Net;
using System.Net.Http.Json;
using InnoClinic.Appointments.API.Application.DTOs;
using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Appointments.API.Infrastructure.Clients
{
    public class ServicesClient(HttpClient httpClient): IServicesClient
    {
        public async Task<int> GetTimeSlotSizeAsync(Guid serviceId, CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/services/{serviceId}/time-slot-size", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException("Service");
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<int>(ct);
            return result;
        }

        public async Task<string?> GetServiceNameAsync(Guid serviceId, CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/services/{serviceId}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var service = await response.Content.ReadFromJsonAsync<ServiceDto>(ct);
            return service?.Name;
        }

        public async Task<string?> GetSpecializationNameAsync(Guid specializationId, CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/specializations/{specializationId}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var spec = await response.Content.ReadFromJsonAsync<SpecializationDto>(ct);
            return spec?.Name;
        }

        public async Task<IReadOnlyDictionary<Guid, string>> GetServiceNamesAsync(
            IEnumerable<Guid> serviceIds,
            CancellationToken ct = default)
        {
            var ids = serviceIds.Distinct().ToList();
            if (ids.Count == 0)
            {
                return new Dictionary<Guid, string>();
            }

            var response = await httpClient.PostAsJsonAsync("/api/v1/services/batch", ids, ct);
            response.EnsureSuccessStatusCode();

            var services = await response.Content.ReadFromJsonAsync<IEnumerable<ServiceDto>>(ct)
                ?? throw new ExternalServiceException("Services.API");

            return services.ToDictionary(s => s.Id, s => s.Name);
        }
    }
}
