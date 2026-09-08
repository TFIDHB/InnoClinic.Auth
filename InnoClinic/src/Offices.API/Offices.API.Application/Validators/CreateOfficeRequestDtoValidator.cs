using FluentValidation;
using InnoClinic.Offices.API.Application.DTOs;

namespace InnoClinic.Offices.API.Application.Validators
{
    public class CreateOfficeRequestDtoValidator : AbstractValidator<CreateOfficeRequestDto>
    {
        public CreateOfficeRequestDtoValidator()
        {
            RuleFor(e => e.Address)
                .NotEmpty();

            RuleFor(e => e.PhotoId)
                .NotEmpty();

            RuleFor(e => e.RegistryPhoneNumber)
                .NotEmpty();
        }
    }
}
