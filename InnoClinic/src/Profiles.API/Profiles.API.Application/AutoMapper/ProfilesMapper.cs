using AutoMapper;
using InnoClinic.Profiles.API.Application.DTOs;
using InnoClinic.Profiles.API.Domain.Entities;

namespace InnoClinic.Profiles.API.Application.AutoMapper
{
    public class ProfilesMapper : Profile
    {
        public ProfilesMapper()
        {
            CreateMap<DoctorProfile, DoctorProfileDto>();
            CreateMap<CreateDoctorProfileRequestDto, DoctorProfile>();
            CreateMap<UpdateDoctorProfileRequestDto, DoctorProfile>();

            CreateMap<PatientProfile, PatientProfileDto>();
            CreateMap<CreatePatientProfileRequestDto, PatientProfile>();
            CreateMap<UpdatePatientProfileRequestDto, PatientProfile>();
            CreateMap<UserAccountInfoDto, PatientProfileDto>();
            CreateMap<CreateMyPatientProfileRequestDto, PatientProfile>();

            CreateMap<ReceptionistProfile, ReceptionistProfileDto>();
            CreateMap<CreateReceptionistProfileRequestDto, ReceptionistProfile>();
            CreateMap<UpdateReceptionistProfileRequestDto, ReceptionistProfile>();
        }
    }
}
