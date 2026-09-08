using InnoClinic.Services.API.Application.DTOs;
using InnoClinic.Services.API.Application.Interfaces;
using System.Net.Http.Json;

namespace InnoClinic.Services.API.Infrastructure.Persistence.Repositories
{
    public class AppointmentsClient(HttpClient httpClient) : IAppointmentsClient
    {
        public async Task<IEnumerable<AppointmentSlotDto>> GetAppointmentsAsync(
            DateOnly date,
            Guid? doctorId,
            CancellationToken ct = default)
        {
            var url = $"/api/v1/appointments?date={date:yyyy-MM-dd}&doctorId={doctorId}";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentSlotDto>>(ct);
            return result ?? throw new InvalidOperationException();
        }

        public async Task<IEnumerable<AppointmentSlotDto>> GetAppointmentsRangeAsync(
            DateOnly startDate,
            DateOnly endDate,
            Guid? doctorId,
            CancellationToken ct = default)
        {
            var url = $"/api/v1/appointments?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&doctorId={doctorId}";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentSlotDto>>(ct);
            return result ?? throw new InvalidOperationException();
        }
    }
}
