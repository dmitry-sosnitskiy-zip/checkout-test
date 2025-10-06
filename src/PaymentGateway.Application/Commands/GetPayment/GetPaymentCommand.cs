using MediatR;

using PaymentGateway.Application.CommandResults;

namespace PaymentGateway.Application.Commands.GetPayment;

public record GetPaymentCommand(
    Guid Id
) : IRequest<CommandResult>;