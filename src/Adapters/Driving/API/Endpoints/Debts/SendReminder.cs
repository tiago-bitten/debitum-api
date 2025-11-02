using API.Extensions;
using API.Infra;
using Application.Debts.Features.SendReminder;
using Application.Shared.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Debts;

internal sealed class SendReminder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("debts/{debtId:guid}/send-reminder", HandleAsync)
            .WithTags(Tags.Debts)
            .WithName("SendPaymentReminder")
            .WithSummary("Send payment reminder to debtor via WhatsApp")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid debtId,
        [FromBody] SendReminderRequest? request,
        [FromServices] ICommandHandler<SendReminderCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new SendReminderCommand(
            debtId,
            request?.CustomMessage
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}

internal sealed record SendReminderRequest(string? CustomMessage);