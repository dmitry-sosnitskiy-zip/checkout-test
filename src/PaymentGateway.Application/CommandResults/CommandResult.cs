namespace PaymentGateway.Application.CommandResults;

public class CommandResult
{
    private CommandResult(CommandResultStatus status, object? data)
    {
        Status = status;
        Data = data;
    }

    public CommandResultStatus Status { get; private set; }

    public object? Data { get; private set; }

    public static CommandResult Success(object? data = null) => new CommandResult(CommandResultStatus.Success, data);

    public static CommandResult Create(CommandResultStatus status, object? data = null) => new CommandResult(status, data);
}