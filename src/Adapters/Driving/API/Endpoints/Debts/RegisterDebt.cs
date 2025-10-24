using API.Extensions;
using API.Infra;
using Application.Debts.Features.RegisterDebt;
using Application.Shared.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Debts;

internal sealed class RegisterDebt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/debts", HandleAsync)
            .WithTags(Tags.Debts)
            .WithName("RegisterDebt")
            .WithSummary("Register a new debt for a debtor")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RegisterDebtRequest request,
        [FromServices] ICommandHandler<RegisterDebtCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new RegisterDebtCommand(
            request.DebtorId,
            request.Amount,
            request.DueDate,
            request.Description
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}

internal sealed record RegisterDebtRequest(
    Guid DebtorId,
    decimal Amount,
    DateTime DueDate,
    string? Description
);