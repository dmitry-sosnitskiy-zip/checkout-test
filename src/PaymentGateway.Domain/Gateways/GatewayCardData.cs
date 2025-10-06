namespace PaymentGateway.Domain.Gateways;

public record GatewayCardData(string CardNumber, string Cvv, int ExpDateMonth, int ExpDateYear);