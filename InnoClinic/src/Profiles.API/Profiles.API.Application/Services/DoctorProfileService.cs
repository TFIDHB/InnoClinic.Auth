using AutoMapper;
using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Profiles.API.Domain.Entities;
using InnoClinic.Profiles.API.Domain.Enums;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Profiles.API.Application.Services
{
    public class DoctorProfileService(
        IProfilesUnitOfWork unitOfWork,
        IMapper mapper,
        IAuthClient authClient): IDoctorProfileService
    {
        public async Task<DoctorProfileDto> CreateAsync(CreateDoctorProfileRequestDto dto, CancellationToken ct = default)
        {
            var accountResult = await authClient.CreateStaffAccountAsync(dto.Email, ct);

            var doctorProfile = mapper.Map<DoctorProfile>(dto);
            doctorProfile.AccountId = accountResult.AccountId;

            await unitOfWork.DoctorProfilesRepository.CreateAsync(doctorProfile, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var responseDto = mapper.Map<DoctorProfileDto>(doctorProfile);

            responseDto.TemporaryPassword = accountResult.TemporaryPassword;

            return responseDto;
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var doctorProfile = await unitOfWork.DoctorProfilesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(DoctorProfile));

            await unitOfWork.DoctorProfilesRepository.DeleteAsync(id, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<DoctorProfileDto>> GetAllAsync(CancellationToken ct = default)
        {
            var doctorProfiles = await unitOfWork.DoctorProfilesRepository.GetAllAsync(ct);
            return mapper.Map<IEnumerable<DoctorProfileDto>>(doctorProfiles);
        }

        public async Task<DoctorProfileDto> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
        {
            var doctorProfile = await unitOfWork.DoctorProfilesRepository.GetByAccountIdAsync(accountId, ct)
                    ?? throw new NotFoundException(nameof(DoctorProfile));

            return mapper.Map<DoctorProfileDto>(doctorProfile);
        }

        public async Task<DoctorProfileDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var doctorProfile = await unitOfWork.DoctorProfilesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(DoctorProfile));

            return mapper.Map<DoctorProfileDto>(doctorProfile);
        }

        public async Task<IEnumerable<DoctorProfileDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var doctors = await unitOfWork.DoctorProfilesRepository.GetByIdsAsync(ids, ct);
            return mapper.Map<IEnumerable<DoctorProfileDto>>(doctors);
        }

        public async Task<IEnumerable<DoctorProfileDto>> GetFilteredDoctorsAsync(
            Guid? specializationId,
            Guid? officeId,
            string? search,
            DoctorStatus? status,
            CancellationToken ct = default)
        {
            var doctors = await unitOfWork.DoctorProfilesRepository.GetFilteredAsync(specializationId, officeId, search, status, ct);
            return mapper.Map<IEnumerable<DoctorProfileDto>>(doctors);
        }

        public async Task<AccountProfileInfoDto?> GetProfileInfoByAccountIdAsync(Guid id, CancellationToken ct = default)
        {
            var doctorProfile = await unitOfWork.DoctorProfilesRepository.GetByAccountIdAsync(id, ct);

            if (doctorProfile == null)
            {
                return null;
            }

            return new AccountProfileInfoDto
            {
                Role = "Doctor",
                Status = doctorProfile.Status.ToString(),
            };
        }

        public async Task<DoctorProfileDto> UpdateAsync(
            Guid id,
            UpdateDoctorProfileRequestDto dto,
            Guid? accountOwnerId = null,
            CancellationToken ct = default)
        {
            var doctorProfile = await unitOfWork.DoctorProfilesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(DoctorProfile));

            if (accountOwnerId.HasValue && accountOwnerId.Value != doctorProfile.AccountId)
            {
                throw new ForbiddenException(ProfilesApplicationMessages.ForbiddenAccessMessage);
            }

            mapper.Map(dto, doctorProfile);
            await unitOfWork.DoctorProfilesRepository.UpdateAsync(doctorProfile, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<DoctorProfileDto>(doctorProfile);
        }
    }
}
