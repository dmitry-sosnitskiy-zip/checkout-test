using FluentValidation;
using MediatR;
using PaymentGateway.Application.CommandResults;

namespace PaymentGateway.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : CommandResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage))
            .ToList();

        if (errors.Any())
        {
            var failedResult = CommandResult.Create(CommandResultStatus.ValidationFailure, new { ValidationErrors = errors });
            if (failedResult is TResponse failedResponseResult)
            {
                return failedResponseResult;
            }
        }

        var response = await next();
        return response;

    }

    public record ValidationError(string MemberName, string? ErrorMessage);
}

