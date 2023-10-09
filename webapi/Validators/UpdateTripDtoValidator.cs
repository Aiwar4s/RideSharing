using FluentValidation;

namespace webapi.Validators
{
    public class UpdateTripDtoValidator : AbstractValidator<UpdateTripDto>
    {
        public UpdateTripDtoValidator() 
        {
            RuleFor(dto => dto.Time).NotEmpty().NotNull().GreaterThan(DateTime.Now.AddHours(12));
            RuleFor(dto => dto.Seats).NotEmpty().NotNull().InclusiveBetween(1, 25);
        }
    }
}
