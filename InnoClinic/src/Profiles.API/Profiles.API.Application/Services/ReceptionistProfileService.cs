using AutoMapper;
using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Profiles.API.Domain.Entities;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Profiles.API.Application.Services
{
    public class ReceptionistProfileService(
        IProfilesUnitOfWork unitOfWork,
        IMapper mapper,
        IAuthClient authClient) : IProfilesService<ReceptionistProfileDto, CreateReceptionistProfileRequestDto, UpdateReceptionistProfileRequestDto>
    {
        public async Task<ReceptionistProfileDto> CreateAsync(CreateReceptionistProfileRequestDto dto, CancellationToken ct = default)
        {
            var accountResult = await authClient.CreateStaffAccountAsync(dto.Email, ct);

            var receptionistProfile = mapper.Map<ReceptionistProfile>(dto);
            receptionistProfile.AccountId = accountResult.AccountId;

            await unitOfWork.ReceptionistProfilesRepository.CreateAsync(receptionistProfile, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var responseDto = mapper.Map<ReceptionistProfileDto>(receptionistProfile);

            responseDto.TemporaryPassword = accountResult.TemporaryPassword;

            return responseDto;
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var receptionistProfile = await unitOfWork.ReceptionistProfilesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(ReceptionistProfile));

            await unitOfWork.ReceptionistProfilesRepository.DeleteAsync(id, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<ReceptionistProfileDto>> GetAllAsync(CancellationToken ct = default)
        {
            var receptionistProfiles = await unitOfWork.ReceptionistProfilesRepository.GetAllAsync(ct);
            return mapper.Map<IEnumerable<ReceptionistProfileDto>>(receptionistProfiles);
        }

        public async Task<ReceptionistProfileDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var receptionistProfile = await unitOfWork.ReceptionistProfilesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(ReceptionistProfile));

            return mapper.Map<ReceptionistProfileDto>(receptionistProfile);
        }

        public async Task<AccountProfileInfoDto?> GetProfileInfoByAccountIdAsync(Guid id, CancellationToken ct = default)
        {
            var receptionistProfile = await unitOfWork.ReceptionistProfilesRepository.GetByAccountIdAsync(id, ct);

            if (receptionistProfile == null)
            {
                return null;
            }

            return new AccountProfileInfoDto { Role = "Receptionist" };
        }

        public async Task<ReceptionistProfileDto> UpdateAsync(
            Guid id,
            UpdateReceptionistProfileRequestDto dto,
            Guid? accountOwnerId = null,
            CancellationToken ct = default)
        {
            var receptionistProfile = await unitOfWork.ReceptionistProfilesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(ReceptionistProfile));

            mapper.Map(dto, receptionistProfile);
            await unitOfWork.ReceptionistProfilesRepository.UpdateAsync(receptionistProfile, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<ReceptionistProfileDto>(receptionistProfile);
        }
    }
}
