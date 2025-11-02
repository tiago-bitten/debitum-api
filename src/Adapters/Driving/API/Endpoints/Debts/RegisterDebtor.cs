using API.Extensions;
using API.Infra;
using Application.Debts.Features.RegisterDebtor;
using Application.Shared.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Debts;

internal sealed class RegisterDebtor : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("debtors", HandleAsync)
            .WithTags(Tags.Debts)
            .WithName("RegisterDebtor")
            .WithSummary("Register a new debtor")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RegisterDebtorRequest request,
        [FromServices] ICommandHandler<RegisterDebtorCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new RegisterDebtorCommand(
            request.CustomerId,
            request.Name,
            request.Phone,
            request.Email
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}

internal sealed record RegisterDebtorRequest(
    Guid CustomerId,
    string Name,
    string Phone,
    string? Email
);
