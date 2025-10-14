using Infra.Mongo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfraMongo(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();