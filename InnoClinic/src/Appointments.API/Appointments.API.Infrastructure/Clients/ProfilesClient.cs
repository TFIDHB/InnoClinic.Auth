using System.Net;
using System.Net.Http.Json;
using InnoClinic.Appointments.API.Application.DTOs;
using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Appointments.API.Infrastructure.Clients
{
    public class ProfilesClient(HttpClient httpClient): IProfilesClient
    {
        public async Task<PatientInfoDto?> GetPatientInfoAsync(Guid patientId, CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/patients/{patientId}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PatientInfoDto>(ct)
                ?? throw new ExternalServiceException("Profiles.API");
        }

        public async Task<DoctorInfoDto?> GetDoctorInfoAsync(Guid doctorId, CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/doctors/{doctorId}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<DoctorInfoDto>(ct)
                ?? throw new ExternalServiceException("Profiles.API");
        }

        public async Task<Guid> GetMyPatientProfileIdAsync(CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/patients/me", ct);
            response.EnsureSuccessStatusCode();

            var profile = await response.Content.ReadFromJsonAsync<PatientInfoDto>(ct)
                ?? throw new ExternalServiceException("Profiles.API");
            return profile.Id;
        }

        public async Task<Guid> GetMyDoctorProfileIdAsync(CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/doctors/me", ct);
            response.EnsureSuccessStatusCode();

            var profile = await response.Content.ReadFromJsonAsync<DoctorInfoDto>(ct)
                ?? throw new ExternalServiceException("Profiles.API");
            return profile.Id;
        }

        public async Task<IReadOnlyDictionary<Guid, PatientInfoDto>> GetPatientsInfoAsync(
            IEnumerable<Guid> patientIds,
            CancellationToken ct = default)
        {
            var ids = patientIds.Distinct().ToList();
            if (ids.Count == 0)
            {
                return new Dictionary<Guid, PatientInfoDto>();
            }

            var response = await httpClient.PostAsJsonAsync("/api/v1/patients/batch", ids, ct);
            response.EnsureSuccessStatusCode();

            var patients = await response.Content.ReadFromJsonAsync<IEnumerable<PatientInfoDto>>(ct)
                ?? throw new ExternalServiceException("Profiles.API");

            return patients.ToDictionary(p => p.Id);
        }

        public async Task<IReadOnlyDictionary<Guid, DoctorInfoDto>> GetDoctorsInfoAsync(
            IEnumerable<Guid> doctorsIds,
            CancellationToken ct = default)
        {
            var ids = doctorsIds.Distinct().ToList();
            if (ids.Count == 0)
            {
                return new Dictionary<Guid, DoctorInfoDto>();
            }

            var response = await httpClient.PostAsJsonAsync("/api/v1/doctors/batch", ids, ct);
            response.EnsureSuccessStatusCode();

            var doctors = await response.Content.ReadFromJsonAsync<IEnumerable<DoctorInfoDto>>(ct)
                ?? throw new ExternalServiceException("Profiles.API");

            return doctors.ToDictionary(p => p.Id);
        }
    }
}
