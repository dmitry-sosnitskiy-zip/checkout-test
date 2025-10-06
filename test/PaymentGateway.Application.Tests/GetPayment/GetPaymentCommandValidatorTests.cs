using FluentValidation.TestHelper;

using PaymentGateway.Application.Commands.GetPayment;

using Shouldly;

using Xunit;

namespace PaymentGateway.Application.Tests.GetPayment;

public class GetPaymentCommandValidatorTests
{
    private readonly GetPaymentCommandValidator _validator = new();

    [Fact]
    public void Should_HaveValidationError_WhenIdIsEmpty()
    {
        var command = new GetPaymentCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_NotHaveValidationError_WhenIdIsValid()
    {
        var command = new GetPaymentCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }
}