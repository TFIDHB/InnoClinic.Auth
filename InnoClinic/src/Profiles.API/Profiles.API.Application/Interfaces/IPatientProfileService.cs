using InnoClinic.Profiles.API.Application.DTOs;

namespace InnoClinic.Profiles.API.Application.Interfaces
{
    public interface IPatientProfileService : IProfilesService<PatientProfileDto, CreatePatientProfileRequestDto, UpdatePatientProfileRequestDto>
    {
        Task<PatientProfileDto> CreateOrMatchProfileAsync(Guid accountId, CreateMyPatientProfileRequestDto dto, CancellationToken ct = default);

        Task<PatientProfileDto> LinkProfileToAccountAsync(Guid profileId, Guid accountId, IPatientFields fields, CancellationToken ct = default);

        Task<PatientProfileDto> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);

        Task<IEnumerable<PatientProfileDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);

        Task<IEnumerable<PatientProfileDto>> GetFilteredPatientsAsync(string? search, CancellationToken ct = default);
    }
}
