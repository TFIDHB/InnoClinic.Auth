using AutoMapper;
using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Exceptions;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Profiles.API.Domain.Entities;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Profiles.API.Application.Services
{
    public class PatientProfileService(
        IProfilesUnitOfWork unitOfWork,
        IMapper mapper,
        IAuthClient authClient) : IPatientProfileService
    {
        public async Task<PatientProfileDto> CreateAsync(CreatePatientProfileRequestDto dto, CancellationToken ct = default)
        {
            var patientProfile = mapper.Map<PatientProfile>(dto);
            patientProfile.IsLinkedToAccount = false;
            await unitOfWork.PatientProfilesRepository.CreateAsync(patientProfile, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<PatientProfileDto>(patientProfile);
        }

        public async Task<PatientProfileDto> CreateOrMatchProfileAsync(
            Guid accountId,
            CreateMyPatientProfileRequestDto dto,
            CancellationToken ct = default)
        {
            var existingProfile = await unitOfWork.PatientProfilesRepository.GetByAccountIdAsync(accountId, ct);
            if (existingProfile != null)
            {
                throw new ProfileAlreadyExistsException();
            }

            var candidates = await unitOfWork.PatientProfilesRepository.GetUnlinkedProfilesAsync(ct);

            var bestMatch = candidates
                .Select(p => new { Profile = p, Score = CalculateMatchScore(p, dto) })
                .Where(x => x.Score >= 13)
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            if (bestMatch != null)
            {
                await authClient.UpdateUserAccountInfoAsync(accountId, new UpdateUserAccountInfoDto { PhoneNumber = dto.PhoneNumber }, ct);

                var matchDto = mapper.Map<PatientProfileDto>(bestMatch.Profile);
                matchDto.PhoneNumber = dto.PhoneNumber;
                return matchDto;
            }

            var newProfile = mapper.Map<PatientProfile>(dto);
            newProfile.AccountId = accountId;
            newProfile.IsLinkedToAccount = true;

            await unitOfWork.PatientProfilesRepository.CreateAsync(newProfile, ct);
            await unitOfWork.SaveChangesAsync(ct);

            await authClient.UpdateUserAccountInfoAsync(accountId, new UpdateUserAccountInfoDto { PhoneNumber = dto.PhoneNumber }, ct);

            var result = mapper.Map<PatientProfileDto>(newProfile);
            result.PhoneNumber = dto.PhoneNumber;
            return result;
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var patientProfile = await unitOfWork.PatientProfilesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(PatientProfile));

            await unitOfWork.PatientProfilesRepository.DeleteAsync(id, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<PatientProfileDto>> GetAllAsync(CancellationToken ct = default)
        {
            var patientProfiles = await unitOfWork.PatientProfilesRepository.GetAllAsync(ct);
            return mapper.Map<IEnumerable<PatientProfileDto>>(patientProfiles);
        }

        public async Task<PatientProfileDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var patientProfile = await unitOfWork.PatientProfilesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(PatientProfile));

            var dto = mapper.Map<PatientProfileDto>(patientProfile);

            var accountInfo = await authClient.GetAccountInfoAsStaffAsync(id, ct);

            if (accountInfo != null)
            {
                mapper.Map(accountInfo, dto);
            }

            return dto;
        }

        public async Task<AccountProfileInfoDto?> GetProfileInfoByAccountIdAsync(Guid id, CancellationToken ct = default)
        {
            var patientProfile = await unitOfWork.PatientProfilesRepository.GetByAccountIdAsync(id, ct);

            if (patientProfile == null)
            {
                return null;
            }

            return new AccountProfileInfoDto { Role = "Patient" };
        }

        public async Task<PatientProfileDto> UpdateAsync(
            Guid id,
            UpdatePatientProfileRequestDto dto,
            Guid? accountOwnerId = null,
            CancellationToken ct = default)
        {
            var patientProfile = await unitOfWork.PatientProfilesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(PatientProfile));

            if (accountOwnerId.HasValue && accountOwnerId.Value != patientProfile.AccountId)
            {
                throw new ForbiddenException(ProfilesApplicationMessages.ForbiddenAccessMessage);
            }

            mapper.Map(dto, patientProfile);
            await unitOfWork.PatientProfilesRepository.UpdateAsync(patientProfile, ct);
            await unitOfWork.SaveChangesAsync(ct);

            if (dto.PhoneNumber != null)
            {
                await authClient.UpdateUserAccountInfoAsync(id, new UpdateUserAccountInfoDto { PhoneNumber = dto.PhoneNumber }, ct);
            }

            var result = mapper.Map<PatientProfileDto>(patientProfile);
            result.PhoneNumber = dto.PhoneNumber;
            return result;
        }

        public async Task<PatientProfileDto> LinkProfileToAccountAsync(
            Guid profileId,
            Guid accountId,
            IPatientFields fields,
            CancellationToken ct = default)
        {
            var profile = await unitOfWork.PatientProfilesRepository.GetByIdAsync(profileId, ct)
                ?? throw new NotFoundException(nameof(PatientProfile));

            if (profile.IsLinkedToAccount)
            {
                throw new BadRequestException(ProfilesApplicationMessages.ProfileAlreadyLinkedMessage);
            }

            if (CalculateMatchScore(profile, fields) < 13)
            {
                throw new BadRequestException(ProfilesApplicationMessages.ProfileDoesNotMatchMessage);
            }

            profile.AccountId = accountId;
            profile.IsLinkedToAccount = true;

            await unitOfWork.PatientProfilesRepository.UpdateAsync(profile, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<PatientProfileDto>(profile);
        }

        public async Task<PatientProfileDto> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
        {
            var patientProfile = await unitOfWork.PatientProfilesRepository.GetByAccountIdAsync(accountId, ct)
                    ?? throw new NotFoundException(nameof(PatientProfile));

            var dto = mapper.Map<PatientProfileDto>(patientProfile);

            var accountInfo = await authClient.GetUserAccountInfoAsync(accountId, ct);

            if (accountInfo != null)
            {
                mapper.Map(accountInfo, dto);
            }

            return dto;
        }

        public async Task<IEnumerable<PatientProfileDto>> GetFilteredPatientsAsync(string? search, CancellationToken ct = default)
        {
            var patients = await unitOfWork.PatientProfilesRepository.GetFilteredAsync(search, ct);
            return mapper.Map<IEnumerable<PatientProfileDto>>(patients);
        }

        public async Task<IEnumerable<PatientProfileDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var patients = await unitOfWork.PatientProfilesRepository.GetByIdsAsync(ids, ct);
            return mapper.Map<IEnumerable<PatientProfileDto>>(patients);
        }

        private static int CalculateMatchScore(PatientProfile profile, IPatientFields dto)
        {
            var score = 0;
            if (string.Equals(profile.FirstName, dto.FirstName, StringComparison.OrdinalIgnoreCase))
            {
                score += 5;
            }

            if (string.Equals(profile.LastName, dto.LastName, StringComparison.OrdinalIgnoreCase))
            {
                score += 5;
            }

            if (profile.MiddleName != null && string.Equals(profile.MiddleName, dto.MiddleName, StringComparison.OrdinalIgnoreCase))
            {
                score += 5;
            }

            if (profile.DateOfBirth == dto.DateOfBirth)
            {
                score += 3;
            }

            return score;
        }
    }
}
