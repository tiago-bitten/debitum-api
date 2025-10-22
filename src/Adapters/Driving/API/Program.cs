using System.Reflection;
using API;
using API.Extensions;
using Application;
using Infra.Postgres;
using Infra.Postgres.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfraPostgres(builder.Configuration)
    .AddApplication()
    .AddAPI();

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DebitumDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapEndpoints();

app.MapGet("/", () => "Hello World!");

await app.RunAsync();

namespace API
{
    public partial class Program
    {
    }
}