using System.Security.Claims;
using AutoMapper;
using InnoClinic.Auth.API.BLL.DTOs;
using InnoClinic.Auth.API.BLL.Exceptions;
using InnoClinic.Auth.API.BLL.Interfaces;
using InnoClinic.Auth.API.DAL.Entities;
using InnoClinic.Auth.API.DAL.Interfaces;
using InnoClinic.Shared.Constants;
using InnoClinic.Shared.Exceptions;
using InnoClinic.Shared.Extensions;

namespace InnoClinic.Auth.API.BLL.Services
{
    public class AuthService(
        ITokenService tokenService,
        IAuthUnitOfWork unitOfWork,
        IMapper mapper,
        IProfilesClient profilesClient,
        IPasswordGenerator passwordGenerator): IAuthService
    {
        public async Task RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default)
        {
            if (await unitOfWork.UserRepository.ExistsByEmailAsync(dto.Email, ct))
            {
                throw new EmailAlreadyExistsException();
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = mapper.Map<User>(dto);
            user.PasswordHash = passwordHash;
            user.CreatedAt = DateTime.UtcNow;

            await unitOfWork.UserRepository.CreateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<AuthTokenDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
        {
            var user = await unitOfWork.UserRepository.GetByEmailAsync(dto.Email, ct);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new InvalidPasswordException();
            }

            var profileInfo = await profilesClient.GetProfileInfoByAccountIdAsync(user.Id, ct);

            var role = profileInfo?.Role ?? Roles.Patient;

            if (profileInfo != null && profileInfo.Role == Roles.Doctor && profileInfo.Status == "Inactive")
            {
                throw new InactiveEntityException();
            }

            var accessToken = tokenService.GenerateAccessToken(user, role);
            var refreshToken = tokenService.GenerateRefreshToken();

            var refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);

            user.RefreshToken = refreshTokenHash;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await unitOfWork.UserRepository.UpdateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return new AuthTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        public async Task LogoutAsync(LogOutRequestDto dto, Guid userId, CancellationToken ct = default)
        {
            var user = await unitOfWork.UserRepository.GetByIdAsync(userId, ct);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (user.RefreshTokenExpiry < DateTime.UtcNow ||
                !BCrypt.Net.BCrypt.Verify(dto.RefreshToken, user.RefreshToken))
            {
                throw new InvalidTokenException();
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<CreateStaffAccountResponseDto> CreateStaffAccountAsync(CreateStaffAccountRequestDto dto, CancellationToken ct = default)
        {
            if (await unitOfWork.UserRepository.ExistsByEmailAsync(dto.Email, ct))
            {
                throw new EmailAlreadyExistsException();
            }

            var temporaryPassword = passwordGenerator.Generate();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
            };

            await unitOfWork.UserRepository.CreateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Temporary decision for now. Password should be sent on email
            return new CreateStaffAccountResponseDto
            {
                AccountId = user.Id,
                TemporaryPassword = temporaryPassword,
            };
        }

        public async Task<UserAccountInfoDto> GetUserAccountInfo(Guid userId, ClaimsPrincipal currentUser, CancellationToken ct = default)
        {
            var currentUserId = currentUser.GetUserId();
            var isStaff = currentUser.IsInRole(Roles.Doctor) || currentUser.IsInRole(Roles.Receptionist);

            if (!isStaff && currentUserId != userId)
            {
                throw new ForbiddenException(BllMessages.ForbiddenAccessMessage);
            }

            var user = await unitOfWork.UserRepository.GetByIdAsync(userId, ct)
                ?? throw new UserNotFoundException();

            return mapper.Map<UserAccountInfoDto>(user);
        }

        public async Task UpdateUserAccountInfo(Guid userId, UpdateUserAccountInfoDto dto, ClaimsPrincipal currentUser, CancellationToken ct = default)
        {
            var currentUserId = currentUser.GetUserId();
            var isInternalService = currentUser.IsInRole(Roles.InternalService);

            if (!isInternalService && currentUserId != userId)
            {
                throw new ForbiddenException(BllMessages.ForbiddenAccessMessage);
            }

            var user = await unitOfWork.UserRepository.GetByIdAsync(userId, ct)
                ?? throw new UserNotFoundException();

            mapper.Map(dto, user);
            await unitOfWork.UserRepository.UpdateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}