using InnoClinic.Offices.API.Application.Interfaces;
using InnoClinic.Offices.API.Infrastructure.Persistance;
using InnoClinic.Offices.API.Infrastructure.Persistance.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Offices.API.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<OfficesDbContext>();
            services.AddScoped<IOfficesRepository, OfficesRepository>();
            return services;
        }
    }
}
