using FluentValidation;

namespace webapi.Validators
{
    public class CreateUserDtoValidator: AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
            RuleFor(dto => dto.Email).NotEmpty().NotNull().EmailAddress();
            RuleFor(dto => dto.Password).NotEmpty().NotNull().Length(min: 5, max: 100);
            RuleFor(dto => dto.PhoneNumber).NotEmpty().NotNull().Matches("[+]370\\d{8}$");
        }
    }
}
