using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Tests;

/// <summary>
/// Simple in-memory test repository that mimics persistence without side effects.
/// </summary>
internal class InMemoryRepositoryForTests : IRepository
{
    private readonly List<Merchant> _merchants = new();
    private readonly List<Payment> _payments = new();

    public Merchant AddMerchant(GatewayType gateway)
    {
        var merchant = new Merchant
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Name = gateway.ToString(),
            Gateway = gateway
        };
        _merchants.Add(merchant);
        return merchant;
    }

    public Task<Merchant?> GetMerchant(Guid id)
    {
        var merchant = _merchants.SingleOrDefault(m => m.Id == id);
        return Task.FromResult(merchant);
    }

    public Task<Payment> AddPayment(Payment payment)
    {
        payment.Id = Guid.NewGuid();
        payment.CreatedDate = DateTime.UtcNow;
        payment.UpdatedDate = DateTime.UtcNow;
        _payments.Add(payment);
        return Task.FromResult(payment);
    }

    public Task<Payment> UpdatePayment(Payment payment)
    {
        var existing = _payments.SingleOrDefault(p => p.Id == payment.Id);
        if (existing == null)
            throw new InvalidOperationException("Payment not found");

        existing.Status = payment.Status;
        existing.UpdatedDate = DateTime.UtcNow;
        return Task.FromResult(existing);
    }

    public Task<Payment?> GetPayment(Guid id)
    {
        var payment = _payments.SingleOrDefault(p => p.Id == id);
        return Task.FromResult(payment);
    }
}