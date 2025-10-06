using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain;

public interface IRepository
{
    Task<Payment> AddPayment(Payment payment);
    Task<Payment> UpdatePayment(Payment payment);
    Task<Payment?> GetPayment(Guid id);
    Task<Merchant?> GetMerchant(Guid id);
}