using FluentValidation;

namespace webapi.Validators
{
    public class CreateTripDtoValidator:AbstractValidator<CreateTripDto>
    {
        public CreateTripDtoValidator()
        {
            RuleFor(dto => dto.Departure).NotEmpty().NotNull().Length(min: 5, max: 30);
            RuleFor(dto => dto.Destination).NotEmpty().NotNull().Length(min: 5, max: 30);
            RuleFor(dto => dto.Time).NotEmpty().NotNull().GreaterThan(DateTime.Now.AddHours(12));
            RuleFor(dto => dto.Seats).NotEmpty().NotNull().InclusiveBetween(1, 25);
        }
    }
}
