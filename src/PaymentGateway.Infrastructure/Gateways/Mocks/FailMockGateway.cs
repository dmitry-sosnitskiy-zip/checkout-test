using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Gateways;

namespace PaymentGateway.Infrastructure.Gateways.Mocks;

public class FailMockGateway : PaymentProcessingGateway
{
    public override GatewayType GatewayType => GatewayType.FailMock;
        
    protected override Task<GatewayResult> ProcessPaymentInternal(Payment payment, GatewayCardData cardData)
    {
        var result = new GatewayResult(Success: false,
            GatewayMessage: "Test fail response",
            GatewayReferenceId: Guid.NewGuid().ToString());

        return Task.FromResult(result);
    }
}