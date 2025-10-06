using System.Text.Json.Serialization;

namespace PaymentGateway.Infrastructure.Gateways.MountebankSimulator.DataContracts;

public record ProcessPaymentRequest(
    [property: JsonPropertyName("card_number")] string CardNumber,
    [property: JsonPropertyName("expiry_date")] string ExpiryDate,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("cvv")] string Cvv
);