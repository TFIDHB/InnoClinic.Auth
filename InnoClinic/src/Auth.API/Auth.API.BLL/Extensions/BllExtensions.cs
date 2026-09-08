using InnoClinic.Auth.API.BLL.AutoMapper;
using InnoClinic.Auth.API.BLL.Clients;
using InnoClinic.Auth.API.BLL.Handlers;
using InnoClinic.Auth.API.BLL.Interfaces;
using InnoClinic.Auth.API.BLL.Options;
using InnoClinic.Auth.API.BLL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InnoClinic.Auth.API.BLL.Extensions
{
    public static class BllExtensions
    {
        public static IServiceCollection AddBll(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(typeof(UserMapper));
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();

            services.Configure<ProfilesApiOptions>(configuration.GetSection("ProfilesApi"));

            services.AddTransient<InternalServiceTokenHandler>();
            services.AddHttpClient<IProfilesClient, ProfilesClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ProfilesApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            }).AddHttpMessageHandler<InternalServiceTokenHandler>();
            return services;
        }
    }
}
