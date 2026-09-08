using FluentValidation;
using InnoClinic.Profiles.API.Application.DTOs;

namespace InnoClinic.Profiles.API.Application.Validators
{
    public class CreatePatientProfileRequestDtoValidator : AbstractValidator<CreatePatientProfileRequestDto>
    {
        public CreatePatientProfileRequestDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(ValidationConstants.NameMaxLength);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(ValidationConstants.NameMaxLength);
            RuleFor(x => x.MiddleName).MaximumLength(ValidationConstants.NameMaxLength);

            RuleFor(x => x.DateOfBirth)
                .Must(d => d is null || d.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage(ProfilesApplicationMessages.DateInFutureMessage)
                .Must(d => d is null || d.Value >= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-ValidationConstants.MinBirthYearsAgo)))
                .WithMessage(string.Format(ProfilesApplicationMessages.DateOfBirthConstraintMessage, ValidationConstants.MinBirthYearsAgo));
        }
    }
}
