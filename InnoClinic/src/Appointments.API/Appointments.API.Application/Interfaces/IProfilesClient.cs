using InnoClinic.Appointments.API.Application.DTOs;

namespace InnoClinic.Appointments.API.Application.Interfaces
{
    public interface IProfilesClient
    {
        Task<PatientInfoDto?> GetPatientInfoAsync(Guid patientId, CancellationToken ct = default);

        Task<DoctorInfoDto?> GetDoctorInfoAsync(Guid doctorId, CancellationToken ct = default);

        Task<Guid> GetMyPatientProfileIdAsync(CancellationToken ct = default);

        Task<Guid> GetMyDoctorProfileIdAsync(CancellationToken ct = default);

        Task<IReadOnlyDictionary<Guid, PatientInfoDto>> GetPatientsInfoAsync(IEnumerable<Guid> patientIds, CancellationToken ct = default);

        Task<IReadOnlyDictionary<Guid, DoctorInfoDto>> GetDoctorsInfoAsync(IEnumerable<Guid> doctorsIds, CancellationToken ct = default);
    }
}
