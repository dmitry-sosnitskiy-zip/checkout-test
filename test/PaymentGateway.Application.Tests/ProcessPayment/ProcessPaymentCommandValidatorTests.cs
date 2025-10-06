using FluentValidation.TestHelper;

using PaymentGateway.Application.Commands.ProcessPayment;

using Shouldly;

using Xunit;

namespace PaymentGateway.Application.Tests.ProcessPayment;

public class ProcessPaymentCommandValidatorTests
{
    private readonly ProcessPaymentCommandValidator _validator = new();

    [Fact]
    public void Should_HaveValidationError_WhenCardNumberIsEmpty()
    {
        var command = CreateValidCommand() with { CardNumber = string.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CardNumber);
    }

    [Theory]
    [InlineData("abcd1234")]
    [InlineData("1234abcd5678")]
    public void Should_HaveValidationError_WhenCardNumberContainsNonDigits(string invalidCard)
    {
        var command = CreateValidCommand() with { CardNumber = invalidCard };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CardNumber)
            .WithErrorMessage("Card number must contain only digits.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Should_HaveValidationError_WhenExpiryMonthInvalid(int month)
    {
        var command = CreateValidCommand() with { ExpiryMonth = month };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpiryMonth);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Should_HaveValidationError_WhenAmountInvalid(int amount)
    {
        var command = CreateValidCommand() with { Amount = amount };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_HaveValidationError_WhenCurrencyNotSupported()
    {
        var command = CreateValidCommand() with { Currency = "JPY" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Currency)
            .WithErrorMessage("Currency must be one of: USD, EUR, GBP");
    }

    [Fact]
    public void Should_HaveValidationError_WhenCvvInvalid()
    {
        var command = CreateValidCommand() with { Cvv = "12" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Cvv)
            .WithErrorMessage("CVV must be 3 or 4 digits.");
    }

    [Fact]
    public void Should_HaveValidationError_WhenExpiryDateNotInFuture()
    {
        var now = DateTime.UtcNow;
        var command = CreateValidCommand() with
        {
            ExpiryMonth = now.Month,
            ExpiryYear = (now.Year % 100) - 1 // Past year
        };

        var result = _validator.TestValidate(command);

        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("expiry date must be in the future"));
    }

    [Fact]
    public void Should_NotHaveErrors_WhenCommandIsValid()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeTrue();
    }

    private static ProcessPaymentCommand CreateValidCommand() =>
        new(
            MerchantId: Guid.NewGuid(),
            CardNumber: "4111111111111111",
            ExpiryMonth: 12,
            ExpiryYear: (DateTime.UtcNow.Year % 100) + 1,
            Currency: "USD",
            Amount: 100,
            Cvv: "123"
        );
}