namespace PaymentGateway.Application.CommandResults;

public enum CommandResultStatus
{
    Success,
    DataNotFound,
    ValidationFailure,
    PaymentDeclined,
    UnexpectedError
}