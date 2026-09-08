using FluentValidation;
using InnoClinic.Profiles.API.Application.DTOs;

namespace InnoClinic.Profiles.API.Application.Validators
{
    public class CreateReceptionistProfileRequestDtoValidator : AbstractValidator<CreateReceptionistProfileRequestDto>
    {
        public CreateReceptionistProfileRequestDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(ValidationConstants.NameMaxLength);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(ValidationConstants.NameMaxLength);
            RuleFor(x => x.MiddleName).MaximumLength(ValidationConstants.NameMaxLength);

            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.OfficeId).NotEmpty();
        }
    }
}
