using InnoClinic.Auth.API.DAL;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Auth.API.Tests.Integration
{
    public class SqlContainerFixture : IAsyncLifetime
    {
        public MsSqlContainer SqlContainer { get; private set; }

        public DbContextOptions<AuthDbContext> ContextOptions { get; private set; }

        [Obsolete]
        public SqlContainerFixture()
        {
            SqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await SqlContainer.StartAsync();

            ContextOptions = new DbContextOptionsBuilder<AuthDbContext>()
                .UseSqlServer(SqlContainer.GetConnectionString())
                .Options;

            using var context = new AuthDbContext(ContextOptions);
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await SqlContainer.DisposeAsync();
        }
    }

    [CollectionDefinition("SqlCollection")]
    public class SqlCollection : ICollectionFixture<SqlContainerFixture>
    {
    }
}
