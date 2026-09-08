using FluentValidation;
using FluentValidation.AspNetCore;
using InnoClinic.Profiles.API.Application;
using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Profiles.API.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.Profiles.API.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IDoctorProfileService, DoctorProfileService>();
            services.AddScoped<IPatientProfileService, PatientProfileService>();
            services.AddScoped<IAccountSearchService, AccountSearchService>();

            services.AddScoped<IProfilesService<ReceptionistProfileDto, CreateReceptionistProfileRequestDto, UpdateReceptionistProfileRequestDto>, ReceptionistProfileService>();
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
            services.AddAutoMapper(AssemblyReference.Assembly);
            return services;
        }
    }
}
