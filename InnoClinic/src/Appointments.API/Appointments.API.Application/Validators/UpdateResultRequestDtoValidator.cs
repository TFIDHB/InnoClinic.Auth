using FluentValidation;
using InnoClinic.Appointments.API.Application.DTOs;

namespace InnoClinic.Appointments.API.Application.Validators
{
    public class UpdateResultRequestDtoValidator : AbstractValidator<UpdateResultRequestDto>
    {
        public UpdateResultRequestDtoValidator()
        {
            RuleFor(x => x.Complaints).NotEmpty().WithMessage(AppointmentsApiMessages.ComplaintsRequiredMessage);
            RuleFor(x => x.Conclusion).NotEmpty().WithMessage(AppointmentsApiMessages.ConclusionIsRequiredMessage);
            RuleFor(x => x.Recommendations).NotEmpty().WithMessage(AppointmentsApiMessages.RecommendationsAreRequiredMessage);
        }
    }
}
