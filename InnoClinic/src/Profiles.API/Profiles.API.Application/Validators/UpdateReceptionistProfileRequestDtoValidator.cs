using FluentValidation;
using InnoClinic.Profiles.API.Application.DTOs;

namespace InnoClinic.Profiles.API.Application.Validators
{
    public class UpdateReceptionistProfileRequestDtoValidator : AbstractValidator<UpdateReceptionistProfileRequestDto>
    {
        public UpdateReceptionistProfileRequestDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(ValidationConstants.NameMaxLength);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(ValidationConstants.NameMaxLength);
            RuleFor(x => x.MiddleName).MaximumLength(ValidationConstants.NameMaxLength);
            RuleFor(x => x.OfficeId).NotEmpty();
        }
    }
}
