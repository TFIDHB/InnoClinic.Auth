using FluentValidation;
using InnoClinic.Auth.API.BLL.DTOs;

namespace InnoClinic.Auth.API.Validators
{
    public class LogoutRequestValidator : AbstractValidator<LogOutRequestDto>
    {
        public LogoutRequestValidator()
        {
            RuleFor(request => request.RefreshToken)
                .NotEmpty();
        }
    }
}
