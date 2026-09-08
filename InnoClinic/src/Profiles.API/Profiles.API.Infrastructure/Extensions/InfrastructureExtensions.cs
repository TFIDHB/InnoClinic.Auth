using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Profiles.API.Infrastructure.Clients;
using InnoClinic.Profiles.API.Infrastructure.Options;
using InnoClinic.Profiles.API.Infrastructure.Persistence;
using InnoClinic.Profiles.API.Infrastructure.Persistence.Repositories;
using InnoClinic.Shared.Handlers;
using InnoClinic.Shared.Migrators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InnoClinic.Profiles.API.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProfilesDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ProfilesConnection"));
            });

            services.AddScoped<IProfilesUnitOfWork, ProfilesUnitOfWork>();
            services.AddScoped<IDoctorProfilesRepository, DoctorProfilesRepository>();
            services.AddScoped<IPatientProfilesRepository, PatientProfilesRepository>();
            services.AddScoped<IReceptionistProfilesRepository, ReceptionistProfilesRepository>();
            services.AddHostedService<DatabaseMigrator<ProfilesDbContext>>();

            services.AddHttpContextAccessor();
            services.AddTransient<AuthHeaderDelegationHandler>();

            services.Configure<AuthApiOptions>(configuration.GetSection("AuthApi"));
            services.AddHttpClient<IAuthClient, AuthClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<AuthApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            }).AddHttpMessageHandler<AuthHeaderDelegationHandler>();

            return services;
        }
    }
}
