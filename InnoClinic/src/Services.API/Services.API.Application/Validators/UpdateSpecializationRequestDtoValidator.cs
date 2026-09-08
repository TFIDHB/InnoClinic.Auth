using FluentValidation;
using InnoClinic.Services.API.Application.DTOs;

namespace InnoClinic.Services.API.Application.Validators
{
    public class UpdateSpecializationRequestDtoValidator : AbstractValidator<UpdateSpecializationRequestDto>
    {
        public UpdateSpecializationRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.ServiceIds)
                .NotNull();

            RuleForEach(x => x.ServiceIds)
                .NotEmpty();
        }
    }
}
