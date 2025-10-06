using System.Text.Json.Serialization;

namespace PaymentGateway.Infrastructure.Gateways.MountebankSimulator.DataContracts;

public record ProcessPaymentResponse(
    [property: JsonPropertyName("authorized")] bool Authorized,
    [property: JsonPropertyName("authorization_code")] string AuthorizationCode
);