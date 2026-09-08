using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Shared.Constants;

namespace InnoClinic.Profiles.API.Application.Services
{
    public class AccountSearchService(IProfilesUnitOfWork unitOfWork) : IAccountSearchService
    {
        public async Task<AccountProfileInfoDto?> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
        {
            if (await unitOfWork.PatientProfilesRepository.GetByAccountIdAsync(accountId, ct) is not null)
            {
                return new AccountProfileInfoDto { Role = Roles.Patient };
            }

            if (await unitOfWork.DoctorProfilesRepository.GetByAccountIdAsync(accountId, ct) is { } doctor)
            {
                return new AccountProfileInfoDto { Role = Roles.Doctor, Status = doctor.Status.ToString() };
            }

            if (await unitOfWork.ReceptionistProfilesRepository.GetByAccountIdAsync(accountId, ct) is not null)
            {
                return new AccountProfileInfoDto { Role = Roles.Receptionist };
            }

            return null;
        }
    }
}
