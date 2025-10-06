using Moq;

using PaymentGateway.Domain;
using PaymentGateway.Domain.Gateways;
using PaymentGateway.Infrastructure.Gateways.MountebankSimulator;
using PaymentGateway.Infrastructure.Gateways.MountebankSimulator.DataContracts;

using Shouldly;

using Xunit;

namespace PaymentGateway.Infrastructure.Tests.Gateways;

public class MountebankSimulatorGatewayTests : GatewayTestsBase
{
    [Fact]
    public async Task ProcessPayment_ShouldReturnSuccessResult_WhenAuthorizedTrue()
    {
        // Arrange
        var httpClientMock = new Mock<IHttpClient>();
        var fakeResponse = new ProcessPaymentResponse(true, "AUTH123");

        httpClientMock
            .Setup(x => x.SendPostRequest<ProcessPaymentResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(fakeResponse);

        var gateway = new MountebankSimulatorGateway(httpClientMock.Object);

        // Act
        var result = await gateway.ProcessPayment(SamplePayment, SampleCardData);

        // Assert
        var expected = new GatewayResult(Success: true,
            GatewayMessage: System.Text.Json.JsonSerializer.Serialize(fakeResponse),
            GatewayReferenceId: "AUTH123");

        result.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnFailureResult_WhenAuthorizedFalse()
    {
        // Arrange
        var httpClientMock = new Mock<IHttpClient>();
        var fakeResponse = new ProcessPaymentResponse(false, "DECLINED999");

        httpClientMock
            .Setup(x => x.SendPostRequest<ProcessPaymentResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(fakeResponse);

        var gateway = new MountebankSimulatorGateway(httpClientMock.Object);

        // Act
        var result = await gateway.ProcessPayment(SamplePayment, SampleCardData);

        // Assert
        var expected = new GatewayResult(Success: false,
            GatewayMessage: System.Text.Json.JsonSerializer.Serialize(fakeResponse),
            GatewayReferenceId: "DECLINED999");

        result.ShouldBeEquivalentTo(expected);
    }
}