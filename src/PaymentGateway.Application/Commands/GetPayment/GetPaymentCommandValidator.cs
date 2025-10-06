using FluentValidation;

namespace PaymentGateway.Application.Commands.GetPayment;

public class GetPaymentCommandValidator : AbstractValidator<GetPaymentCommand>
{
    public GetPaymentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}