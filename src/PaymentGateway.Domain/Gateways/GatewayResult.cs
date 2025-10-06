namespace PaymentGateway.Domain.Gateways;

public record GatewayResult(string? GatewayReferenceId, string GatewayMessage, bool Success);