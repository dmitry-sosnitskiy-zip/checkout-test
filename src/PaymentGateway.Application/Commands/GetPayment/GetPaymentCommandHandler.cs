using MediatR;
using PaymentGateway.Application.CommandResults;
using PaymentGateway.Application.Commands.ProcessPayment;
using PaymentGateway.Domain;

namespace PaymentGateway.Application.Commands.GetPayment;

public class GetPaymentCommandHandler(IRepository repository) : IRequestHandler<GetPaymentCommand, CommandResult>
{
    public async Task<CommandResult> Handle(
        GetPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var payment = await repository.GetPayment(command.Id);
        if (payment == null)
        {
            return CommandResult.Create(CommandResultStatus.DataNotFound, $"Payment {command.Id} not found");
        }

        var result = ProcessPaymentResult.FromPayment(payment!);
        
        return CommandResult.Success(result);
    }
}