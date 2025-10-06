using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Gateways;

namespace PaymentGateway.Infrastructure.Gateways.Mocks;

public class SuccessMockGateway : PaymentProcessingGateway
{
    public override GatewayType GatewayType => GatewayType.SuccessMock;
        
    protected override Task<GatewayResult> ProcessPaymentInternal(Payment payment, GatewayCardData cardData)
    {
        var result = new GatewayResult(Success: true,
            GatewayMessage: "Test success response",
            GatewayReferenceId: Guid.NewGuid().ToString());

        return Task.FromResult(result);
    }
}