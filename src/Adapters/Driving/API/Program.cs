using System.Reflection;
using API;
using API.Extensions;
using Infra.Mongo;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfraMongo(builder.Configuration)
    .AddAPI();

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapEndpoints();

app.MapGet("/", () => "Hello World!");

await app.RunAsync();

namespace API
{
    public partial class Program
    {
    }
}