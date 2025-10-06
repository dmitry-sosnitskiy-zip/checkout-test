using System.Net;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Application.CommandResults;
using PaymentGateway.Application.Commands.GetPayment;
using PaymentGateway.Application.Commands.ProcessPayment;

using Shouldly;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerUnitTests
{
    [Fact]
    public async Task GetPaymentAsync_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var payload = new { Id = id, Amount = 123 };
        var commandResult = CommandResult.Success(payload);

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.Is<GetPaymentCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResult);

        var controller = new PaymentsController(mediator.Object);

        // Act
        var result = await controller.GetPaymentAsync(id, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        ok!.Value.ShouldBe(payload);

        mediator.Verify(m => m.Send(It.Is<GetPaymentCommand>(c => c.Id == id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsNotFound_WhenMediatorReturnsDataNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var message = $"Payment {id} not found";
        var commandResult = CommandResult.Create(CommandResultStatus.DataNotFound, message);

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<GetPaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResult);

        var controller = new PaymentsController(mediator.Object);

        // Act
        var result = await controller.GetPaymentAsync(id, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundObjectResult>();
        var nf = result as NotFoundObjectResult;
        nf!.Value.ShouldBe(message);

        mediator.Verify(m => m.Send(It.Is<GetPaymentCommand>(c => c.Id == id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MakePaymentAsync_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var command = new ProcessPaymentCommand(
            merchantId,
            CardNumber: "4111111111111111",
            ExpiryMonth: 12,
            ExpiryYear: 2030,
            Currency: "USD",
            Amount: 100,
            Cvv: "123"
        );

        var dto = new { PaymentId = Guid.NewGuid(), Status = "Authorized" };
        var commandResult = CommandResult.Success(dto);

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.Is<ProcessPaymentCommand>(c =>
                c.MerchantId == command.MerchantId &&
                c.Amount == command.Amount &&
                c.CardNumber == command.CardNumber
            ), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResult);

        var controller = new PaymentsController(mediator.Object);

        // Act
        var result = await controller.MakePaymentAsync(command, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        ok!.Value.ShouldBe(dto);

        mediator.Verify(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MakePaymentAsync_ReturnsBadRequest_WhenMediatorReturnsValidationFailure()
    {
        // Arrange
        var command = new ProcessPaymentCommand(
            Guid.NewGuid(),
            CardNumber: "invalid",
            ExpiryMonth: 0,
            ExpiryYear: 0,
            Currency: "",
            Amount: -1,
            Cvv: ""
        );

        var validationPayload = new { Errors = new[] { "Amount must be > 0" } };
        var commandResult = CommandResult.Create(CommandResultStatus.ValidationFailure, validationPayload);

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResult);

        var controller = new PaymentsController(mediator.Object);

        // Act
        var result = await controller.MakePaymentAsync(command, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var bad = result as BadRequestObjectResult;
        bad!.Value.ShouldBe(validationPayload);

        mediator.Verify(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MakePaymentAsync_ReturnsInternalServerError_WhenMediatorReturnsUnexpectedError()
    {
        // Arrange
        var command = new ProcessPaymentCommand(
            Guid.NewGuid(),
            CardNumber: "4111111111111111",
            ExpiryMonth: 1,
            ExpiryYear: 2025,
            Currency: "USD",
            Amount: 10,
            Cvv: "000"
        );

        var payload = "unexpected failure";
        var commandResult = CommandResult.Create(CommandResultStatus.UnexpectedError, payload);

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResult);

        var controller = new PaymentsController(mediator.Object);

        // Act
        var result = await controller.MakePaymentAsync(command, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ObjectResult>();
        var obj = result as ObjectResult;
        obj!.StatusCode.ShouldBe(500);
        obj.Value.ShouldBe(payload);

        mediator.Verify(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPaymentAsync_ShouldReturn404_WhenPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}