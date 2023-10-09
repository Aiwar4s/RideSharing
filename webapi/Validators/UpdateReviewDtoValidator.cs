using FluentValidation;

namespace webapi.Validators
{
    public class UpdateReviewDtoValidator:AbstractValidator<UpdateReviewDto>
    {
        public UpdateReviewDtoValidator()
        {
            RuleFor(dto => dto.Rating).NotEmpty().NotNull().InclusiveBetween(1, 5);
        }
    }
}
