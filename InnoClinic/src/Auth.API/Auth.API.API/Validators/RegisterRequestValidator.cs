using FluentValidation;
using InnoClinic.Auth.API.BLL.DTOs;

namespace InnoClinic.Auth.API.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage(ApiMessages.EmailRequired)
                .EmailAddress().WithMessage(ApiMessages.EmailInvalid);

            RuleFor(request => request.Password)
                .NotEmpty().WithMessage(ApiMessages.PasswordRequired)
                .Length(6, 15);
        }
    }
}
