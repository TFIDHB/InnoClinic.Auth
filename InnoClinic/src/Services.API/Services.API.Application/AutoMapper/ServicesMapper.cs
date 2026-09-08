using AutoMapper;
using InnoClinic.Services.API.Application.DTOs;
using InnoClinic.Services.API.Domain.Entities;

namespace InnoClinic.Services.API.Application.AutoMapper
{
    public class ServicesMapper : Profile
    {
        public ServicesMapper()
        {
            CreateMap<CreateServiceRequestDto, Service>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateServiceRequestDto, Service>();
            CreateMap<UpdateServiceStatusRequestDto, Service>();
            CreateMap<Service, ServiceDto>();

            CreateMap<CreateSpecializationRequestDto, Specialization>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateSpecializationRequestDto, Specialization>();
            CreateMap<UpdateSpecializationStatusRequestDto, Specialization>();
            CreateMap<Specialization, SpecializationDto>();
        }
    }
}
