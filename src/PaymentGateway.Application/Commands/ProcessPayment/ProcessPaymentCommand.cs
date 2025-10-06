using MediatR;

using PaymentGateway.Application.CommandResults;

namespace PaymentGateway.Application.Commands.ProcessPayment;

public record ProcessPaymentCommand(
    Guid MerchantId,
    string CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    string Cvv
) : IRequest<CommandResult>;