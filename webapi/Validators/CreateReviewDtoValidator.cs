using FluentValidation;

namespace webapi.Validators
{
    public class CreateReviewDtoValidator:AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(dto=>dto.Rating).NotEmpty().NotNull().InclusiveBetween(1, 5);
            RuleFor(dto=>dto.ReviewerId).NotEmpty().NotNull();
        }
    }
}
