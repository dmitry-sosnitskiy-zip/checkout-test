using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Gateways;

public class PaymentProcessingGatewayFactory(IEnumerable<PaymentProcessingGateway> gateways)
{
    public PaymentProcessingGateway LoadGateway(GatewayType gatewayType)
    {
        return gateways.SingleOrDefault(x => x.GatewayType == gatewayType) ??
               throw new NotSupportedException($"Gateway type '{gatewayType}' is not supported");
    }
}