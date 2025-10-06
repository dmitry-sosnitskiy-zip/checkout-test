using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Gateways;

namespace PaymentGateway.Infrastructure.Tests.Gateways;

public class GatewayTestsBase
{
    protected Payment SamplePayment => new()
    {
        Id = Guid.NewGuid(),
        Amount = 150,
        CurrencyCode = "USD",
        Status = PaymentStatus.New,
        Gateway = GatewayType.MountebankSimulator,
        CardData = new CardData
        {
            MaskedNumber = "1111",
            ExpDateMonth = 12,
            ExpDateYear = 30
        },
        MerchantId = Guid.NewGuid()
    };

    protected GatewayCardData SampleCardData => new("4111111111111111", "123", 12, 30);
}