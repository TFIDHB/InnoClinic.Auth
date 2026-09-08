using FluentValidation;
using InnoClinic.Appointments.API.Application.DTOs;
using InnoClinic.Appointments.API.Application.Options;
using Microsoft.Extensions.Options;

namespace InnoClinic.Appointments.API.Application.Validators
{
    public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequestDto>
    {
        public CreateAppointmentRequestValidator(IOptions<WorkingHoursOptions> workingHoursOpt)
        {
            var workingHours = workingHoursOpt.Value;

            RuleFor(x => x.PatientId)
                .NotEmpty();
            RuleFor(x => x.SpecializationId)
                .NotEmpty();
            RuleFor(x => x.DoctorId)
                .NotEmpty();
            RuleFor(x => x.ServiceId)
                .NotEmpty();
            RuleFor(x => x.OfficeId)
                .NotEmpty();
            RuleFor(x => x.Date)
                .NotEmpty()
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(AppointmentsApiMessages.AppointmentInPastMessage);
            RuleFor(x => x.Time)
                .NotEmpty()
                .Must(time => time >= workingHours.Start && time < workingHours.End)
                .WithMessage(string.Format(AppointmentsApiMessages.AppointmentBetweenMessage, workingHours.Start, workingHours.End));
        }
    }
}
