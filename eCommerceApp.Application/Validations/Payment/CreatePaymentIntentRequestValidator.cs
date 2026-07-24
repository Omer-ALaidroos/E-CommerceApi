using eCommerceApp.Application.DTOs.Payment;
using FluentValidation;

namespace eCommerceApp.Application.Validations.Payment
{
    public class CreatePaymentIntentRequestValidator : AbstractValidator<CreatePaymentIntentRequestDto>
    {
        public CreatePaymentIntentRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Order id must be greater than zero.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency must be a 3-letter code.");
        }
    }
}
