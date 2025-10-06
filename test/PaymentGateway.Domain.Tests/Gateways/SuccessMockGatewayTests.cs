using PaymentGateway.Infrastructure.Gateways.Mocks;

using Shouldly;

using Xunit;

namespace PaymentGateway.Infrastructure.Tests.Gateways;

public class SuccessMockGatewayTests : GatewayTestsBase
{
    [Fact]
    public async Task ProcessPayment_ShouldReturnSuccessResult()
    {
        // Arrange
        var gateway = new SuccessMockGateway();

        // Act
        var result = await gateway.ProcessPayment(SamplePayment, SampleCardData);

        // Assert
        result.Success.ShouldBeTrue();
        result.GatewayMessage.ShouldContain("Test success response");
        result.GatewayReferenceId.ShouldNotBeNullOrEmpty();
    }

}