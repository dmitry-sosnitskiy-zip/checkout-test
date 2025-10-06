using PaymentGateway.Infrastructure.Gateways.Mocks;

using Shouldly;

using Xunit;

namespace PaymentGateway.Infrastructure.Tests.Gateways;

public class FailMockGatewayTests : GatewayTestsBase
{
    [Fact]
    public async Task ProcessPayment_ShouldReturnFailureResult()
    {
        // Arrange
        var gateway = new FailMockGateway();

        // Act
        var result = await gateway.ProcessPayment(SamplePayment, SampleCardData);

        // Assert
        result.Success.ShouldBeFalse();
        result.GatewayMessage.ShouldContain("Test fail response");
        result.GatewayReferenceId.ShouldNotBeNullOrEmpty();
    }
}