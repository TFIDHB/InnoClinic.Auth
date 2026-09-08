using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Appointments.API.Infrastructure.Clients;
using InnoClinic.Appointments.API.Infrastructure.Options;
using InnoClinic.Appointments.API.Infrastructure.Persistence;
using InnoClinic.Appointments.API.Infrastructure.Persistence.Repositories;
using InnoClinic.Shared.Handlers;
using InnoClinic.Shared.Migrators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InnoClinic.Appointments.API.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppointmentDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("AppointmentsConnection"));
            });

            services.Configure<ServicesApiOptions>(configuration.GetSection("ServicesApi"));

            services.AddHttpClient<IServicesClient, ServicesClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServicesApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            });

            services.AddHttpContextAccessor();
            services.AddTransient<AuthHeaderDelegationHandler>();
            services.Configure<ProfilesApiOptions>(configuration.GetSection("ProfilesApi"));

            services.AddHttpClient<IProfilesClient, ProfilesClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ProfilesApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            }).AddHttpMessageHandler<AuthHeaderDelegationHandler>();

            services.Configure<DocumentsApiOptions>(configuration.GetSection("DocumentsApi"));
            services.AddHttpClient<IDocumentsClient, DocumentsClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<DocumentsApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            }).AddHttpMessageHandler<AuthHeaderDelegationHandler>();

            services.AddScoped<IAppointmentUnitOfWork, AppointmentUnitOfWork>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IResultRepository, ResultRepository>();
            services.AddHostedService<DatabaseMigrator<AppointmentDbContext>>();
            return services;
        }
    }
}
