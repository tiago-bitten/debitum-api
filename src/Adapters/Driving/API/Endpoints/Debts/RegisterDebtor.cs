using Application.Debts.Features.RegisterDebtor;
using Application.Shared.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Debts;

internal sealed class RegisterDebtor : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/debts/debtors", HandleAsync)
            .WithTags(Tags.Debts)
            .WithName("RegisterDebtor")
            .WithSummary("Register a new debtor")
            .Produces<RegisterDebtorResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RegisterDebtorRequest request,
        [FromServices] ICommandHandler<RegisterDebtorCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new RegisterDebtorCommand(
            request.CustomerId,
            request.Name,
            request.Phone,
            request.Email
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/debts/debtors/{result.Value}", new RegisterDebtorResponse(result.Value))
            : Results.BadRequest(new { Error = result.Error.Description });
    }
}

internal sealed record RegisterDebtorRequest(
    Guid CustomerId,
    string Name,
    string Phone,
    string? Email
);

internal sealed record RegisterDebtorResponse(Guid DebtorId);