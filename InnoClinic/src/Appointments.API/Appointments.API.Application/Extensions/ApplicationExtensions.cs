using FluentValidation;
using FluentValidation.AspNetCore;
using InnoClinic.Appointments.API.Application;
using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Appointments.API.Application.Options;
using InnoClinic.Appointments.API.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Appointments.API.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<WorkingHoursOptions>(configuration.GetSection("WorkingHours"));
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddAutoMapper(AssemblyReference.Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
            return services;
        }
    }
}
