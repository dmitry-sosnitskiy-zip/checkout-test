using PaymentGateway.Application.CommandResults;
using PaymentGateway.Application.Commands.ProcessPayment;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Gateways;
using PaymentGateway.Infrastructure.Gateways.Mocks;

using Shouldly;

using Xunit;

namespace PaymentGateway.Application.Tests.ProcessPayment;

public class ProcessPaymentCommandHandlerTests
{
    private readonly InMemoryRepositoryForTests _repository = new();
    private readonly PaymentProcessingGatewayFactory _gatewayFactory = new([new SuccessMockGateway(), new FailMockGateway()]);

    [Fact]
    public async Task Handle_ShouldReturnDataNotFound_WhenMerchantDoesNotExist()
    {
        // Arrange
        var command = new ProcessPaymentCommand(
            MerchantId: Guid.NewGuid(),
            CardNumber: "4111111111111111",
            ExpiryMonth: 12,
            ExpiryYear: 30,
            Currency: "USD",
            Amount: 100,
            Cvv: "123");

        var handler = new ProcessPaymentCommandHandler(repository: _repository, gatewayFactory: _gatewayFactory);

        // Act
        var result = await handler.Handle(command: command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(CommandResultStatus.DataNotFound);
    }

    [Fact]
    public async Task Handle_ShouldProcessPaymentSuccessfully_WhenGatewayApproves()
    {
        // Arrange
        var merchant = _repository.AddMerchant(GatewayType.SuccessMock);

        var command = new ProcessPaymentCommand(
            MerchantId: merchant.Id,
            CardNumber: "4111111111111111",
            ExpiryMonth: 12,
            ExpiryYear: 30,
            Currency: "USD",
            Amount: 150,
            Cvv: "321");

        var handler = new ProcessPaymentCommandHandler(repository: _repository, gatewayFactory: _gatewayFactory);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(CommandResultStatus.Success);

        var paymentResult = result.Data.ShouldBeOfType<ProcessPaymentResult>();
        paymentResult.Status.ShouldBe(PaymentStatus.Authorized);
        paymentResult.Currency.ShouldBe("USD");
        paymentResult.Amount.ShouldBe(150);
    }

    [Fact]
    public async Task Handle_ShouldProcessPaymentAsDeclined_WhenGatewayRejects()
    {
        // Arrange
        var merchant = _repository.AddMerchant(GatewayType.FailMock);

        var command = new ProcessPaymentCommand(
            MerchantId: merchant.Id,
            CardNumber: "4111111111111111",
            ExpiryMonth: 10,
            ExpiryYear: 30,
            Currency: "EUR",
            Amount: 250,
            Cvv: "999");

        var handler = new ProcessPaymentCommandHandler(repository: _repository, gatewayFactory: _gatewayFactory);

        // Act
        var result = await handler.Handle(command: command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(CommandResultStatus.PaymentDeclined);

        var paymentResult = result.Data.ShouldBeOfType<ProcessPaymentResult>();
        paymentResult.Status.ShouldBe(PaymentStatus.Declined);
        paymentResult.Currency.ShouldBe("EUR");
        paymentResult.Amount.ShouldBe(250);
    }
}