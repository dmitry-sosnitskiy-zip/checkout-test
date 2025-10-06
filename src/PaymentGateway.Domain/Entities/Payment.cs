namespace PaymentGateway.Domain.Entities;

public class Payment : Entity
{
    public int Amount { get; set; }

    public string Description { get; set; }

    public GatewayType Gateway { get; set; }

    public string? GatewayReferenceId { get; set; }

    public CardData CardData { get; set; }

    public PaymentStatus Status { get; set; }

    public Guid MerchantId { get; set; }

    public string GatewayResponseMessage { get; set; }
        
    public string CurrencyCode { get; set; }
}