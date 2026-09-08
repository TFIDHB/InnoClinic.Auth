using System.Net;
using System.Net.Http.Json;
using InnoClinic.Auth.API.BLL.DTOs;
using InnoClinic.Auth.API.BLL.Interfaces;
using InnoClinic.Auth.API.DAL;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.API.Tests.Integration
{
    [Collection("SqlCollection")]
    public class AuthControllerIntegrationTests : IAsyncLifetime
    {
        private readonly SqlContainerFixture _fixture;
        private HttpClient _client;
        private WebApplicationFactory<Program> _factory;

        public AuthControllerIntegrationTests(SqlContainerFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(e => e.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        services.AddDbContext<AuthDbContext>(opt => opt.UseSqlServer(_fixture.SqlContainer.GetConnectionString()));

                        var profilesDescriptor = services.SingleOrDefault(e => e.ServiceType == typeof(IProfilesClient));
                        if (profilesDescriptor != null)
                        {
                            services.Remove(profilesDescriptor);
                        }

                        services.AddScoped<IProfilesClient, FakeProfilesClient>();
                    });
                });

            _client = _factory.CreateClient();

            using var context = new AuthDbContext(_fixture.ContextOptions);
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            _client?.Dispose();
            await _factory.DisposeAsync();
        }

        [Fact]
        public async Task Register_WhenUserDataIsValid_ReturnsOk()
        {
            var email = "test@test.com";
            var dto = new RegisterRequestDto { Email = email, Password = "123456" };

            var result = await _client.PostAsJsonAsync("api/v1/auth/register", dto);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var userInDb = await db.Users.FirstOrDefaultAsync(e => e.Email == email);
            Assert.NotNull(userInDb);
        }

        [Fact]
        public async Task Register_WhenUserDataIsInvalid_ReturnsBadRequest()
        {
            var dto = new RegisterRequestDto { Email = "1111", Password = "123456" };

            var result = await _client.PostAsJsonAsync("api/v1/auth/register", dto);

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Register_WhenEmailAlreadyExists_ReturnsBadRequest()
        {
            var email = "duplicate@test.com";
            var dto = new RegisterRequestDto { Email = email, Password = "123456" };
            await _client.PostAsJsonAsync("api/v1/auth/register", dto);

            var result = await _client.PostAsJsonAsync("api/v1/auth/register", dto);

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Login_WhenUserCredentalsCorrect_ReturnsOkAndToken()
        {
            var email = "login@test.com";
            var password = "123456";

            await _client.PostAsJsonAsync("api/v1/auth/register", new RegisterRequestDto { Email = email, Password = password });
            var result = await _client.PostAsJsonAsync("api/v1/auth/login", new LoginRequestDto { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var tokens = await result.Content.ReadFromJsonAsync<AuthTokenDto>();
            Assert.NotNull(tokens);
        }

        [Fact]
        public async Task Login_WhenUserDoesNotExist_ReturnsBadRequest()
        {
            var result = await _client.PostAsJsonAsync("api/v1/auth/login", new LoginRequestDto { Email = "doesnotexist@test.com", Password = "123456" });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Login_WhenPasswordIsIncorrect_ReturnsBadRequest()
        {
            var email = "wrongpassword@test.com";

            await _client.PostAsJsonAsync("api/v1/auth/register", new RegisterRequestDto { Email = email, Password = "123456" });
            var result = await _client.PostAsJsonAsync("api/v1/auth/login", new LoginRequestDto { Email = email, Password = "wrong-password" });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Logout_WhenUserIsAuthorized_ReturnsOk()
        {
            var email = "logout@test.com";
            var password = "123456";

            await _client.PostAsJsonAsync("api/v1/auth/register", new RegisterRequestDto { Email = email, Password = password });
            var loginResult = await _client.PostAsJsonAsync("api/v1/auth/login", new LoginRequestDto { Email = email, Password = password });
            var tokens = await loginResult.Content.ReadFromJsonAsync<AuthTokenDto>();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var result = await _client.PostAsJsonAsync("api/v1/auth/logout", new LogOutRequestDto { RefreshToken = tokens.RefreshToken });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var userInDb = await db.Users.FirstOrDefaultAsync(e => e.Email == email);
            Assert.Null(userInDb.RefreshToken);
            Assert.Null(userInDb.RefreshTokenExpiry);
        }

        [Fact]
        public async Task Logout_WhenUserIsUnauthorized_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var result = await _client.PostAsJsonAsync("api/v1/auth/logout", new LogOutRequestDto { RefreshToken = string.Empty });

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task Logout_WhenRefreshTokenIsInvalid_ReturnsBadRequest()
        {
            var email = "invalidrefreshtoken@test.com";
            var password = "123456";

            await _client.PostAsJsonAsync("api/v1/auth/register", new RegisterRequestDto { Email = email, Password = password });
            var loginResult = await _client.PostAsJsonAsync("api/v1/auth/login", new LoginRequestDto { Email = email, Password = password });
            var tokens = await loginResult.Content.ReadFromJsonAsync<AuthTokenDto>();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var result = await _client.PostAsJsonAsync("api/v1/auth/logout", new LogOutRequestDto { RefreshToken = "wrong-refresh-token" });

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}
