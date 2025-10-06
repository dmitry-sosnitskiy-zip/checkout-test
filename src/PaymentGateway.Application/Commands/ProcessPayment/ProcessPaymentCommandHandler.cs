using MediatR;
using PaymentGateway.Application.CommandResults;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Gateways;

namespace PaymentGateway.Application.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler(IRepository repository, PaymentProcessingGatewayFactory gatewayFactory) : IRequestHandler<ProcessPaymentCommand, CommandResult>
{
    public async Task<CommandResult> Handle(
        ProcessPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var merchant = await repository.GetMerchant(command.MerchantId);
        if (merchant == null)
        {
            return CommandResult.Create(CommandResultStatus.DataNotFound, $"Merchant {command.MerchantId} not found");
        }
        
        var payment = new Payment
        {
            Amount = command.Amount,
            CurrencyCode = command.Currency,
            Gateway = merchant.Gateway,
            Status = PaymentStatus.New,
            MerchantId = merchant.Id,
            CardData = new CardData
            {
                MaskedNumber = command.CardNumber[^4..],
                ExpDateMonth = command.ExpiryMonth,
                ExpDateYear = command.ExpiryYear,
            }
        };

        await repository.AddPayment(payment);

        var gateway = gatewayFactory.LoadGateway(merchant.Gateway);

        var gatewayCardData = new GatewayCardData(command.CardNumber, command.Cvv, command.ExpiryMonth, command.ExpiryYear);

        var gatewayResult = await gateway.ProcessPayment(payment, gatewayCardData);

        payment.GatewayReferenceId = gatewayResult.GatewayReferenceId;
        payment.GatewayResponseMessage = gatewayResult.GatewayMessage;
        payment.Status = gatewayResult.Success ? PaymentStatus.Authorized : PaymentStatus.Declined;
        await repository.UpdatePayment(payment);

        var result = ProcessPaymentResult.FromPayment(payment);

        return payment.Status == PaymentStatus.Authorized
            ? CommandResult.Success(result)
            : CommandResult.Create(CommandResultStatus.PaymentDeclined, result);
    }
}