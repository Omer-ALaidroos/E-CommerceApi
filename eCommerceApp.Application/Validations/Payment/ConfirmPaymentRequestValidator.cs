using eCommerceApp.Application.DTOs.Payment;
using FluentValidation;

namespace eCommerceApp.Application.Validations.Payment
{
    public class ConfirmPaymentRequestValidator : AbstractValidator<ConfirmPaymentRequestDto>
    {
        public ConfirmPaymentRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Order id must be greater than zero.");

            RuleFor(x => x.PaymentIntentId)
                .NotEmpty().WithMessage("Payment intent id is required.");

            RuleFor(x => x.PaymentMethodId)
                .NotEmpty().WithMessage("Payment method id is required.");
        }
    }
}
