using InnoClinic.Appointments.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Appointments.API.Tests.Integration
{
    public class PostgresContainerFixture : IAsyncLifetime
    {
        public PostgreSqlContainer PostgresContainer { get; private set; }

        public DbContextOptions<AppointmentDbContext> ContextOptions { get; private set; } = null!;

        [Obsolete]
        public PostgresContainerFixture()
        {
            PostgresContainer = new PostgreSqlBuilder()
                .WithImage("postgres:17")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await PostgresContainer.StartAsync();

            ContextOptions = new DbContextOptionsBuilder<AppointmentDbContext>()
                .UseNpgsql(PostgresContainer.GetConnectionString())
                .Options;

            using var context = new AppointmentDbContext(ContextOptions);
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await PostgresContainer.DisposeAsync();
        }
    }

    [CollectionDefinition("PostgresCollection")]
    public class PostgresCollection : ICollectionFixture<PostgresContainerFixture>
    {
    }
}