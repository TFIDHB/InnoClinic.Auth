using AutoMapper;
using InnoClinic.Auth.API.BLL.DTOs;
using InnoClinic.Auth.API.BLL.Exceptions;
using InnoClinic.Auth.API.BLL.Interfaces;
using InnoClinic.Auth.API.BLL.Services;
using InnoClinic.Auth.API.DAL.Entities;
using InnoClinic.Auth.API.DAL.Interfaces;
using Moq;

namespace Auth.API.Tests.Unit
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IProfilesClient> _profilesClientMock;
        private readonly Mock<IPasswordGenerator> _passwordGeneratorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _unitOfWorkMock = new Mock<IAuthUnitOfWork>();
            _tokenServiceMock = new Mock<ITokenService>();
            _mapperMock = new Mock<IMapper>();
            _profilesClientMock = new Mock<IProfilesClient>();
            _passwordGeneratorMock = new Mock<IPasswordGenerator>();
            _authService = new AuthService(
                _tokenServiceMock.Object,
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _profilesClientMock.Object,
                _passwordGeneratorMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsEmailAlreadyExistsException()
        {
            var dto = new RegisterRequestDto { Email = "test@test.com", Password = "123456" };
            _unitOfWorkMock
                .Setup(e => e.UserRepository.ExistsByEmailAsync(dto.Email, default))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EmailAlreadyExistsException>(
                async () => await _authService.RegisterAsync(dto));
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailIsNew_CreatesUserAndSavesChanges()
        {
            var dto = new RegisterRequestDto { Email = "test@test.com", Password = "123456" };
            _unitOfWorkMock
                .Setup(e => e.UserRepository.ExistsByEmailAsync(dto.Email, default))
                .ReturnsAsync(false);
            _mapperMock
                .Setup(e => e.Map<User>(dto))
                .Returns(new User { Id = Guid.NewGuid(), Email = dto.Email, PasswordHash = "placeholder" });

            await _authService.RegisterAsync(dto);

            _unitOfWorkMock.Verify(e => e.UserRepository.CreateAsync(It.Is<User>(x => x.Email == dto.Email), default), Times.Once);
            _unitOfWorkMock.Verify(e => e.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ThrowsUserNotFoundException()
        {
            var dto = new LoginRequestDto { Email = "test@test.com", Password = "123456" };
            _unitOfWorkMock
                .Setup(e => e.UserRepository.GetByEmailAsync(dto.Email, default))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UserNotFoundException>(
                async () => await _authService.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsIncorrect_ThrowsInvalidPasswordException()
        {
            var dto = new LoginRequestDto { Email = "test@test.com", Password = "123456" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("incorrect-password"),
            };
            _unitOfWorkMock
                .Setup(e => e.UserRepository.GetByEmailAsync(dto.Email, default))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<InvalidPasswordException>(
                async () => await _authService.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_WhenCredentialsAreValid_ReturnsAuthTokenDto()
        {
            var dto = new LoginRequestDto { Email = "test@test.com", Password = "123456" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            };
            var role = "Patient";

            _unitOfWorkMock
                .Setup(e => e.UserRepository.GetByEmailAsync(dto.Email, default))
                .ReturnsAsync(user);
            _tokenServiceMock
                .Setup(e => e.GenerateAccessToken(user, role))
                .Returns("access-token");
            _tokenServiceMock
                .Setup(e => e.GenerateRefreshToken())
                .Returns("refresh-token");

            var result = await _authService.LoginAsync(dto, default);

            Assert.NotNull(result);
            Assert.Equal("access-token", result.AccessToken);
            Assert.Equal("refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task LogoutAsync_WhenUserNotFound_ThrowsUserNotFoundException()
        {
            var dto = new LogOutRequestDto { RefreshToken = "some-token" };
            var userId = Guid.NewGuid();
            _unitOfWorkMock
                .Setup(e => e.UserRepository.GetByIdAsync(userId, default))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UserNotFoundException>(
                async () => await _authService.LogoutAsync(dto, userId));
        }

        [Fact]
        public async Task LogoutAsync_WhenTokenIsInvalid_ThrowsTokenIsInvalidException()
        {
            var dto = new LogOutRequestDto { RefreshToken = "wrong-token" };
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test",
                PasswordHash = "placeholder",
                RefreshToken = BCrypt.Net.BCrypt.HashPassword("correct-token"),
                RefreshTokenExpiry = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(10),
            };
            _unitOfWorkMock
                .Setup(e => e.UserRepository.GetByIdAsync(userId, default))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<InvalidTokenException>(
                async () => await _authService.LogoutAsync(dto, userId));
        }

        [Fact]
        public async Task LogoutAsync_WhenTokenIsExpired_ThrowsTokenIsInvalidException()
        {
            var dto = new LogOutRequestDto { RefreshToken = "correct-token" };
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test",
                PasswordHash = "placeholder",
                RefreshToken = BCrypt.Net.BCrypt.HashPassword("correct-token"),
                RefreshTokenExpiry = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            _unitOfWorkMock
                .Setup(e => e.UserRepository.GetByIdAsync(userId, default))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<InvalidTokenException>(
                async () => await _authService.LogoutAsync(dto, userId));
        }

        [Fact]
        public async Task LogoutAsync_WhenTokenIsValid_ClearsRefreshToken()
        {
            var dto = new LogOutRequestDto { RefreshToken = "correct-token" };
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test",
                PasswordHash = "placeholder",
                RefreshToken = BCrypt.Net.BCrypt.HashPassword("correct-token"),
                RefreshTokenExpiry = new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(10),
            };
            _unitOfWorkMock
                .Setup(e => e.UserRepository.GetByIdAsync(userId, default))
                .ReturnsAsync(user);

            await _authService.LogoutAsync(dto, userId);

            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiry);
        }
    }
}
