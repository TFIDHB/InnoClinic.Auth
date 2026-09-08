using InnoClinic.Auth.API.DAL;
using InnoClinic.Auth.API.DAL.Entities;
using InnoClinic.Auth.API.DAL.Repositories;

namespace Auth.API.Tests.Integration
{
    [Collection("SqlCollection")]
    public class UserRepositoryIntegrationTests : IAsyncLifetime
    {
        private readonly SqlContainerFixture _fixture;

        public UserRepositoryIntegrationTests(SqlContainerFixture fixture)
        {
            _fixture = fixture;
        }

        private AuthDbContext CreateContext() => new (_fixture.ContextOptions);

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailExists_ReturnsTrue()
        {
            var email = "exists@test.com";
            using (var context = CreateContext())
            {
                await context.Users.AddAsync(new User { Id = Guid.NewGuid(), Email = email, PasswordHash = "passwordHash" });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);
                var result = await repository.ExistsByEmailAsync(email);
                Assert.True(result);
            }
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailDoesNotExist_ReturnsFalse()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var result = await repository.ExistsByEmailAsync("doesnotexists@test.com");

            Assert.False(result);
        }

        [Fact]
        public async Task GetByEmailAsync_WhenEmailExists_ReturnsUser()
        {
            var email = "getbyemail@test.com";
            var expectedUser = new User { Id = Guid.NewGuid(), Email = email, PasswordHash = "passwordHash" };
            using (var context = CreateContext())
            {
                await context.Users.AddAsync(expectedUser);
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);
                var actualUser = await repository.GetByEmailAsync(email);
                Assert.NotNull(actualUser);
                Assert.Equal(expectedUser.Email, actualUser.Email);
                Assert.Equal(expectedUser.PasswordHash, actualUser.PasswordHash);
            }
        }

        [Fact]
        public async Task GetByEmailAsync_WhenEmailDoesNotExist_ReturnsNull()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var result = await repository.GetByEmailAsync("noemail@test.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_WhenTokenExists_ReturnsUser()
        {
            var refreshToken = "valid-refresh-token";
            var expectedUser = new User { Id = Guid.NewGuid(), Email = "owner@test.com", PasswordHash = "passwordHash", RefreshToken = refreshToken };
            using (var context = CreateContext())
            {
                await context.Users.AddAsync(expectedUser);
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);
                var actualUser = await repository.GetByRefreshTokenAsync(refreshToken);
                Assert.NotNull(actualUser);
                Assert.Equal(expectedUser.Email, actualUser.Email);
                Assert.Equal(refreshToken, actualUser.RefreshToken);
            }
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_WhenTokenDoesNotExist_ReturnsNull()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var result = await repository.GetByRefreshTokenAsync("fake-refresh-token");

            Assert.Null(result);
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
