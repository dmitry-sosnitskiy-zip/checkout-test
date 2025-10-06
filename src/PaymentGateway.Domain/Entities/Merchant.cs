namespace PaymentGateway.Domain.Entities;

public class Merchant : Entity
{
    public string Name { get; set; }

    public GatewayType Gateway { get; set; }
}