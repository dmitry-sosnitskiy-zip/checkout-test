using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Infrastructure;

public class InMemoryRepository : IRepository
{
    private readonly List<Payment> _payments = new();
    private readonly List<Merchant> _merchants = new();

    public InMemoryRepository()
    {
        SeedTestMerchant();
    }

    public Task<Payment> AddPayment(Payment payment)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        payment.Id = Guid.NewGuid();
        payment.CreatedDate = DateTime.UtcNow;
        payment.UpdatedDate = DateTime.UtcNow;

        _payments.Add(payment);

        return Task.FromResult(payment);
    }

    public Task<Payment> UpdatePayment(Payment payment)
    {
        var existingPayment = _payments.SingleOrDefault(x => x.Id == payment.Id);
        if (existingPayment == null)
        {
            throw new InvalidOperationException($"Payment with ID {payment.Id} does not exist.");
        }

        existingPayment.Amount = payment.Amount;
        existingPayment.Description = payment.Description;
        existingPayment.Gateway = payment.Gateway;
        existingPayment.GatewayReferenceId = payment.GatewayReferenceId;
        existingPayment.CardData = payment.CardData;
        existingPayment.Status = payment.Status;
        existingPayment.MerchantId = payment.MerchantId;
        existingPayment.GatewayResponseMessage = payment.GatewayResponseMessage;
        existingPayment.CurrencyCode = payment.CurrencyCode;
        existingPayment.UpdatedDate = DateTime.UtcNow;

        return Task.FromResult(existingPayment);
    }

    public Task<Payment?> GetPayment(Guid id)
    {
        var payment = _payments.SingleOrDefault(x => x.Id == id);

        return Task.FromResult(payment);
    }

    public Task<Merchant?> GetMerchant(Guid id)
    {
        var merchant = _merchants.SingleOrDefault(x => x.Id == id);

        return Task.FromResult(merchant);
    }

    private void SeedTestMerchant()
    {
        _merchants.Add(new Merchant
        {
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Gateway = GatewayType.MountebankSimulator,
        });
    }
}