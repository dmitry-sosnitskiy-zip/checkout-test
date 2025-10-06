using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.CommandResults;
using PaymentGateway.Application.Commands.GetPayment;
using PaymentGateway.Application.Commands.ProcessPayment;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IMediator mediator) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPaymentCommand(id), cancellationToken);
        
        return CreateActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> MakePaymentAsync([FromBody] ProcessPaymentCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return CreateActionResult(result);
    }

    private IActionResult CreateActionResult(CommandResult commandResult)
    {
        return commandResult.Status switch
        {
            CommandResultStatus.Success => Ok(commandResult.Data),
            CommandResultStatus.DataNotFound => NotFound(commandResult.Data),
            CommandResultStatus.ValidationFailure => BadRequest(commandResult.Data),
            CommandResultStatus.PaymentDeclined => StatusCode(402, commandResult.Data),
            CommandResultStatus.UnexpectedError => StatusCode(500, commandResult.Data),
            _ => throw new NotSupportedException($"Command result status '{commandResult.Status}' is not supported.")
        };
    }
}