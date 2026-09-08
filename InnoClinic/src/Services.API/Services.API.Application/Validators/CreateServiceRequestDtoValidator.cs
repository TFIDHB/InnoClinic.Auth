using Application;
using FluentValidation;
using InnoClinic.Services.API.Application.DTOs;

namespace InnoClinic.Services.API.Application.Validators
{
    public class CreateServiceRequestDtoValidator : AbstractValidator<CreateServiceRequestDto>
    {
        public CreateServiceRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage(ServiceMessages.InvalidPriceMessage);

            RuleFor(x => x.ServiceCategoryId)
                .NotEmpty();
        }
    }
}
