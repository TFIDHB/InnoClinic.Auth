using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Domain.Enums;

namespace InnoClinic.Profiles.API.Application.Interfaces
{
    public interface IDoctorProfileService : IProfilesService<DoctorProfileDto, CreateDoctorProfileRequestDto, UpdateDoctorProfileRequestDto>
    {
        Task<DoctorProfileDto> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);

        Task<IEnumerable<DoctorProfileDto>> GetFilteredDoctorsAsync(
            Guid? specializationId,
            Guid? officeId,
            string? search,
            DoctorStatus? status,
            CancellationToken ct = default);

        Task<IEnumerable<DoctorProfileDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    }
}
