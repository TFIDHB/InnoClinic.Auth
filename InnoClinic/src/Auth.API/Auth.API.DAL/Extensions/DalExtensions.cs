using InnoClinic.Auth.API.DAL.Interfaces;
using InnoClinic.Auth.API.DAL.Repositories;
using InnoClinic.Shared.Migrators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Auth.API.DAL.Extensions
{
    public static class DalExtensions
    {
        public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("AuthConnection")));

            services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddHostedService<DatabaseMigrator<AuthDbContext>>();
            return services;
        }
    }
}
