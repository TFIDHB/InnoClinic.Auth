using AutoMapper;
using InnoClinic.Appointments.API.Application.DTOs;
using InnoClinic.Appointments.API.Domain.Entities;

namespace InnoClinic.Appointments.API.Application.AutoMapper
{
    public class AppointmentMapper : Profile
    {
        public AppointmentMapper()
        {
            CreateMap<CreateAppointmentRequestDto, Appointment>()
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Duration, opt => opt.Ignore());

            CreateMap<Appointment, AppointmentResponseDto>();
            CreateMap<Appointment, AppointmentSlotDto>();

            CreateMap<Appointment, ScheduleDto>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Time.Add(src.Duration)))
                .ForMember(dest => dest.PatientFullName, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceName, opt => opt.Ignore())
                .ForMember(dest => dest.HasResult, opt => opt.MapFrom(src => false));

            CreateMap<Appointment, AppointmentListItemDto>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Time.Add(src.Duration)))
                .ForMember(dest => dest.DoctorFullName, opt => opt.Ignore())
                .ForMember(dest => dest.PatientFullName, opt => opt.Ignore())
                .ForMember(dest => dest.PatientPhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceName, opt => opt.Ignore());

            CreateMap<Appointment, AppointmentHistoryItemDto>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Time.Add(src.Duration)))
                .ForMember(dest => dest.DoctorFullName, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceName, opt => opt.Ignore())
                .ForMember(dest => dest.HasResult, opt => opt.Ignore())
                .ForMember(dest => dest.CanReschedule, opt => opt.Ignore());

            CreateMap<CreateResultRequestDto, Result>();
            CreateMap<UpdateResultRequestDto, Result>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AppointmentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Result, ResultDto>()
                .ForMember(dest => dest.Date, opt => opt.Ignore())
                .ForMember(dest => dest.PatientFullName, opt => opt.Ignore())
                .ForMember(dest => dest.PatientDateOfBirth, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorFullName, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorSpecialization, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceName, opt => opt.Ignore())
                .ForMember(dest => dest.CanEdit, opt => opt.Ignore());
        }
    }
}
