using AutoMapper;
using InnoClinic.Auth.API.BLL.AutoMapper;
using InnoClinic.Auth.API.BLL.DTOs;
using InnoClinic.Auth.API.BLL.Exceptions;
using InnoClinic.Auth.API.BLL.Interfaces;
using InnoClinic.Auth.API.BLL.Services;
using InnoClinic.Auth.API.DAL;
using InnoClinic.Auth.API.DAL.Interfaces;
using InnoClinic.Auth.API.DAL.Repositories;
using InnoClinic.Shared.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Auth.API.Tests.Integration
{
    public class FakeProfilesClient : IProfilesClient
    {
        public Task<AccountProfileInfoDto?> GetProfileInfoByAccountIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<AccountProfileInfoDto?>(null);
    }

    public class ServiceAssemblyResult
    {
        public required AuthDbContext DbContext { get; set; }

        public required AuthService AuthService { get; set; }
    }

    [Collection("SqlCollection")]
    public class AuthServiceIntegrationTests : IAsyncLifetime
    {
        private static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private readonly SqlContainerFixture _fixture;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;

        public AuthServiceIntegrationTests(SqlContainerFixture fixture)
        {
            _fixture = fixture;

            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserMapper>()).CreateMapper();

            var jwtSettings = Options.Create(new JwtSettings
            {
                Secret = "test-secret-key-for-jwt-settings",
                Issuer = "test",
                Audience = "test",
                ExpirationMinutes = 5,
            });
            _tokenService = new TokenService(jwtSettings);
        }

        private ServiceAssemblyResult CreateServiceContext()
        {
            var dbContext = new AuthDbContext(_fixture.ContextOptions);
            var userRepository = new UserRepository(dbContext);

            var services = new ServiceCollection();
            services.AddScoped<IUserRepository>(_ => userRepository);
            var serviceProvider = services.BuildServiceProvider();

            var unitOfWork = new AuthUnitOfWork(dbContext, serviceProvider);
            var profilesClient = new FakeProfilesClient();
            var passwordGenerator = new PasswordGenerator();

            var authService = new AuthService(_tokenService, unitOfWork, _mapper, profilesClient, passwordGenerator);

            return new ServiceAssemblyResult
            {
                DbContext = dbContext,
                AuthService = authService,
            };
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailIsNew_AddUserToDatabase()
        {
            var assembly = CreateServiceContext();
            var email = "test@test.com";
            var dto = new RegisterRequestDto { Email = email, Password = "123456" };

            await assembly.AuthService.RegisterAsync(dto);

            assembly.DbContext.ChangeTracker.Clear();
            var user = await assembly.DbContext.Users.FirstOrDefaultAsync(e => e.Email == email);
            Assert.NotNull(user);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsEmailAlreadyExistsException()
        {
            var assembly = CreateServiceContext();
            var email = "duplicate@test.com";
            var dto = new RegisterRequestDto { Email = email, Password = "123456" };

            await assembly.AuthService.RegisterAsync(dto);

            await Assert.ThrowsAsync<EmailAlreadyExistsException>(async () =>
            {
                await assembly.AuthService.RegisterAsync(dto);
            });
        }

        [Fact]
        public async Task LoginAsync_WhenCredentialsAreValid_ReturnsTokens()
        {
            var assembly = CreateServiceContext();
            var email = "login@test.com";
            var password = "123456";

            var registerDto = new RegisterRequestDto { Email = email, Password = password };
            await assembly.AuthService.RegisterAsync(registerDto);
            var loginDto = new LoginRequestDto { Email = email, Password = password };

            var tokens = await assembly.AuthService.LoginAsync(loginDto);

            Assert.NotNull(tokens);
            Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
            Assert.False(string.IsNullOrEmpty(tokens.RefreshToken));
        }

        [Fact]
        public async Task LoginAsync_WhenUserDoesNotExist_ThrowsUserNotFoundException()
        {
            var assembly = CreateServiceContext();
            var loginDto = new LoginRequestDto { Email = "doesnotexist@test.com", Password = "123456" };

            await Assert.ThrowsAsync<UserNotFoundException>(async () =>
            {
                await assembly.AuthService.LoginAsync(loginDto);
            });
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsIncorrect_ThrowsInvalidPasswordException()
        {
            var assembly = CreateServiceContext();
            var email = "wrongpassword@test.com";

            var registerDto = new RegisterRequestDto { Email = email, Password = "123456" };
            await assembly.AuthService.RegisterAsync(registerDto);
            var loginDto = new LoginRequestDto { Email = email, Password = "wrongPassword" };

            await Assert.ThrowsAsync<InvalidPasswordException>(async () =>
            {
                await assembly.AuthService.LoginAsync(loginDto);
            });
        }

        [Fact]
        public async Task LogoutAsync_WhenDataIsValid_ClearsRefreshTokenInDatabase()
        {
            var assembly = CreateServiceContext();
            var email = "logout@test.com";
            var password = "123456";

            var registerDto = new RegisterRequestDto { Email = email, Password = password };
            await assembly.AuthService.RegisterAsync(registerDto);
            var loginDto = new LoginRequestDto { Email = email, Password = password };
            var tokens = await assembly.AuthService.LoginAsync(loginDto);
            assembly.DbContext.ChangeTracker.Clear();
            var user = await assembly.DbContext.Users.FirstAsync(u => u.Email == email);
            var logoutDto = new LogOutRequestDto { RefreshToken = tokens.RefreshToken };

            await assembly.AuthService.LogoutAsync(logoutDto, user.Id);

            assembly.DbContext.ChangeTracker.Clear();
            var userAfterLogout = await assembly.DbContext.Users.FirstAsync(u => u.Id == user.Id);
            Assert.Null(userAfterLogout.RefreshToken);
            Assert.Null(userAfterLogout.RefreshTokenExpiry);
        }

        [Fact]
        public async Task LogoutAsync_WhenUserDoesNotExist_ThrowsUserNotFoundException()
        {
            var assembly = CreateServiceContext();
            var logoutDto = new LogOutRequestDto { RefreshToken = "some-token" };

            await Assert.ThrowsAsync<UserNotFoundException>(async () =>
            {
                await assembly.AuthService.LogoutAsync(logoutDto, userId: TestUserId);
            });
        }

        [Fact]
        public async Task LogoutAsync_WhenRefreshTokenIsInvalid_ThrowsInvalidTokenException()
        {
            var assembly = CreateServiceContext();
            var email = "invalidtoken@test.com";
            var password = "123456";

            var registerDto = new RegisterRequestDto { Email = email, Password = password };
            await assembly.AuthService.RegisterAsync(registerDto);
            var loginDto = new LoginRequestDto { Email = email, Password = password };
            await assembly.AuthService.LoginAsync(loginDto);

            assembly.DbContext.ChangeTracker.Clear();
            var user = await assembly.DbContext.Users.FirstAsync(u => u.Email == email);
            var logoutDto = new LogOutRequestDto { RefreshToken = "wrong-token" };

            await Assert.ThrowsAsync<InvalidTokenException>(async () =>
            {
                await assembly.AuthService.LogoutAsync(logoutDto, user.Id);
            });
        }

        public async Task InitializeAsync()
        {
            using var context = new AuthDbContext(_fixture.ContextOptions);
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}