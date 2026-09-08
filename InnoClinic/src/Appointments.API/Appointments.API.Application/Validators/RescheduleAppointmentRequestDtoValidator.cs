using FluentValidation;
using InnoClinic.Appointments.API.Application.DTOs;

namespace InnoClinic.Appointments.API.Application.Validators
{
    public class RescheduleAppointmentRequestDtoValidator : AbstractValidator<RescheduleAppointmentRequestDto>
    {
        public RescheduleAppointmentRequestDtoValidator()
        {
            RuleFor(x => x.DoctorId).NotEmpty();
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Time).NotEmpty();
            RuleFor(x => x)
                .Must(x => x.Date.ToDateTime(x.Time) > DateTime.UtcNow)
                .WithMessage(AppointmentsApiMessages.AppointmentInPastMessage);
        }
    }
}
