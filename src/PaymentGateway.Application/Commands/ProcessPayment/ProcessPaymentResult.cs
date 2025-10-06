using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Commands.ProcessPayment;

public record ProcessPaymentResult(
    Guid Id,
    PaymentStatus Status,
    string LastFourDigits,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount)
{
    public static ProcessPaymentResult FromPayment(Payment payment) =>
        new(
            Id: payment.Id,
            Status: payment.Status,
            LastFourDigits: payment.CardData.MaskedNumber,
            ExpiryMonth: payment.CardData.ExpDateMonth,
            ExpiryYear: payment.CardData.ExpDateYear,
            Currency: payment.CurrencyCode,
            Amount: payment.Amount
        );
}