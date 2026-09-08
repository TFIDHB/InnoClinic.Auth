using FluentValidation;
using InnoClinic.Offices.API.Application.DTOs;

namespace InnoClinic.Offices.API.Application.Validators
{
    public class UpdateOfficeRequestDtoValidator : AbstractValidator<UpdateOfficeRequestDto>
    {
        public UpdateOfficeRequestDtoValidator()
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
