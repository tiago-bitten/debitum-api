using System.Reflection;
using API;
using API.Extensions;
using Application;
using Infra.Postgres;
using Infra.Postgres.Shared.Persistence;
using Infra.WhatsApp;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfraPostgres(builder.Configuration)
    .AddInfraWhatsApp(builder.Configuration)
    .AddApplication()
    .AddAPI();

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Debitum API v1");
    options.RoutePrefix = string.Empty;
});

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