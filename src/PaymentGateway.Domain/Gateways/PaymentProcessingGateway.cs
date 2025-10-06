using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Gateways;

public abstract class PaymentProcessingGateway
{
    public abstract GatewayType GatewayType { get; }

    public async Task<GatewayResult> ProcessPayment(Payment payment, GatewayCardData cardData)
    {
        try
        {
            return await ProcessPaymentInternal(payment, cardData);
        }
        catch (Exception e)
        {
            return new GatewayResult(
                Success: false,
                GatewayMessage: $"Unhandled exception: {e}",
                GatewayReferenceId: null);
        }
    }

    protected abstract Task<GatewayResult> ProcessPaymentInternal(Payment payment, GatewayCardData cardData);
}