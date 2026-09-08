using InnoClinic.Services.API.Application.Interfaces;
using InnoClinic.Services.API.Infrastructure.Options;
using InnoClinic.Services.API.Infrastructure.Persistence;
using InnoClinic.Services.API.Infrastructure.Persistence.Repositories;
using InnoClinic.Shared.Migrators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InnoClinic.Services.API.Infrastructure.Extensions
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ServicesDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ServicesConnection"));
            });

            services.Configure<AppointmentsApiOptions>(configuration.GetSection("AppointmentsApi"));

            services.AddHttpClient<IAppointmentsClient, AppointmentsClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<AppointmentsApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            });

            services.AddScoped<IServicesUnitOfWork, ServicesUnitOfWork>();
            services.AddScoped<IServicesRepository, ServicesRepository>();
            services.AddScoped<ISpecializationsRepository, SpecializationsRepository>();
            services.AddHostedService<DatabaseMigrator<ServicesDbContext>>();
            return services;
        }
    }
}
