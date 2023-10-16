using FluentValidation;

namespace webapi.Validators
{
    public class CreateDriverDtoValidator: AbstractValidator<CreateDriverDto>
    {
        public CreateDriverDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
            RuleFor(dto => dto.Email).NotEmpty().NotNull().EmailAddress();
            RuleFor(dto => dto.PhoneNumber).NotEmpty().NotNull().Matches("[+]370\\d{8}$");
        }
    }
}
