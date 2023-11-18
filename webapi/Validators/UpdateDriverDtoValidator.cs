using FluentValidation;

namespace webapi.Validators
{
    public class UpdateDriverDtoValidator: AbstractValidator<UpdateDriverDto>
    {
        public UpdateDriverDtoValidator()
        {
            RuleFor(dto => dto.Email).NotEmpty().NotNull().EmailAddress();
            RuleFor(dto => dto.PhoneNumber).NotEmpty().NotNull().Matches("[+]370\\d{8}$");
        }
    }
}
