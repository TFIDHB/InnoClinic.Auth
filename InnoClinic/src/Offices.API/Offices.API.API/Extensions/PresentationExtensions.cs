using InnoClinic.Shared.Extensions;
using InnoClinic.Shared.Options;

namespace InnoClinic.Offices.API.Extensions
{
    public static class PresentationExtensions
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOptions = configuration
                .GetSection("Cors")
                .Get<CorsOptions>()!;

            services.AddControllers();
            services
                .AddAppSwagger("InnoClinic.Offices.API")
                .AddOpenApi();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            return services;
        }
    }
}
