using PaymentGateway.Application.CommandResults;
using PaymentGateway.Application.Commands.GetPayment;
using PaymentGateway.Application.Commands.ProcessPayment;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;

using Shouldly;

using Xunit;

namespace PaymentGateway.Application.Tests.GetPayment;

public class GetPaymentCommandHandlerTests
{
    private readonly InMemoryRepositoryForTests _repository = new InMemoryRepositoryForTests();

    [Fact]
    public async Task Handle_ShouldReturnDataNotFound_WhenPaymentDoesNotExist()
    {
        // Arrange
        var command = new GetPaymentCommand(Guid.NewGuid());
        var handler = new GetPaymentCommandHandler(_repository);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(CommandResultStatus.DataNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnPayment_WhenPaymentExists()
    {
        // Arrange
        var existingPayment = await _repository.AddPayment(new Payment
        {
            Amount = 250,
            CurrencyCode = "USD",
            Gateway = GatewayType.SuccessMock,
            Status = PaymentStatus.Authorized,
            MerchantId = Guid.NewGuid(),
            CardData = new CardData
            {
                MaskedNumber = "1234",
                ExpDateMonth = 12,
                ExpDateYear = 30
            }
        });

        var command = new GetPaymentCommand(existingPayment.Id);
        var handler = new GetPaymentCommandHandler(_repository);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(CommandResultStatus.Success);

        var paymentResult = result.Data.ShouldBeOfType<ProcessPaymentResult>();
        paymentResult.ShouldBeEquivalentTo(new ProcessPaymentResult(
            Id: existingPayment.Id,
            Status: existingPayment.Status,
            LastFourDigits: existingPayment.CardData.MaskedNumber,
            ExpiryMonth: existingPayment.CardData.ExpDateMonth,
            ExpiryYear: existingPayment.CardData.ExpDateYear,
            Currency: existingPayment.CurrencyCode,
            Amount: existingPayment.Amount));
    }
}