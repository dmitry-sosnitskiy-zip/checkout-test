﻿using FluentValidation;

namespace PaymentGateway.Application.Commands.ProcessPayment;

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    private static readonly string[] AllowedCurrencies = { "USD", "EUR", "GBP" };

    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Length(14, 19)
            .Matches(@"^\d+$").WithMessage("Card number must contain only digits.");

        RuleFor(x => x.ExpiryMonth)
            .InclusiveBetween(1, 12);

        RuleFor(x => x.ExpiryYear)
            .InclusiveBetween(0, 99);

        RuleFor(x => new { x.ExpiryMonth, x.ExpiryYear })
            .Must(BeInTheFuture)
            .WithMessage("The card expiry date must be in the future.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Must(c => AllowedCurrencies.Contains(c))
            .WithMessage("Currency must be one of: " + string.Join(", ", AllowedCurrencies));

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Cvv)
            .NotEmpty()
            .Matches(@"^\d{3,4}$").WithMessage("CVV must be 3 or 4 digits.");
    }

    private static bool BeInTheFuture(dynamic expiry)
    {
        try
        {
            var now = DateTime.UtcNow;
            // Assume 2-digit year: 0–49 maps to 2000–2049, 50–99 maps to 1950–1999 (adjust as needed)
            var century = expiry.ExpiryYear < 50 ? 2000 : 1900;
            var year = century + expiry.ExpiryYear;
            var lastDayOfExpiryMonth = new DateTime(
                year,
                expiry.ExpiryMonth,
                DateTime.DaysInMonth(year, expiry.ExpiryMonth)
            );

            return lastDayOfExpiryMonth >= now.Date;
        }
        catch (ArgumentOutOfRangeException)
        {
            // Invalid month/day values
            return false;
        }
    }}