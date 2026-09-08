using FluentValidation;
using FluentValidation.AspNetCore;
using InnoClinic.Services.API.Application;
using InnoClinic.Services.API.Application.Interfaces;
using InnoClinic.Services.API.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Services.API.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<ISpecializationsService, SpecializationsService>();
            services.AddAutoMapper(AssemblyReference.Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
            return services;
        }
    }
}
