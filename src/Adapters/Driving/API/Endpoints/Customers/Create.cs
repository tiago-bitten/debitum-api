using API.Extensions;
using API.Infra;
using Application.Customers.Features.CreateCustomer;
using Application.Shared.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Customers;

internal sealed class Create : IEndpoint
{
    public sealed record Request(string Name, string Email, string Phone);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("customers", async (
                [FromBody] Request request,
                [FromServices] ICommandHandler<CreateCustomerCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateCustomerCommand(request.Name, request.Email, request.Phone);

                var result = await handler.HandleAsync(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.Customers);
    }
}