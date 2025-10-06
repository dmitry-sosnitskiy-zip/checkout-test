namespace PaymentGateway.Domain.Entities;

public class CardData : Entity
{
    public string MaskedNumber { get; set; }

    public int ExpDateMonth { get; set; }

    public int ExpDateYear { get; set; }
}