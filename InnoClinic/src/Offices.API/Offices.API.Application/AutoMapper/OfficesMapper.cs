using AutoMapper;
using InnoClinic.Offices.API.Application.DTOs;
using InnoClinic.Offices.API.Domain.Entities;

namespace InnoClinic.Offices.API.Application.AutoMapper
{
    public class OfficesMapper : Profile
    {
        public OfficesMapper()
        {
            CreateMap<CreateOfficeRequestDto, Office>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            CreateMap<UpdateOfficeRequestDto, Office>();
            CreateMap<UpdateOfficeStatusRequestDto, Office>();
            CreateMap<Office, OfficeDto>();
        }
    }
}
