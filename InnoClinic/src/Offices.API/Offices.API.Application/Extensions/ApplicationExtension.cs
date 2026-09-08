using FluentValidation;
using FluentValidation.AspNetCore;
using InnoClinic.Offices.API.Application;
using InnoClinic.Offices.API.Application.Interfaces;
using InnoClinic.Offices.API.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Offices.API.Application.Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IOfficesService, OfficesService>();
            services.AddAutoMapper(AssemblyReference.Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
            return services;
        }
    }
}
