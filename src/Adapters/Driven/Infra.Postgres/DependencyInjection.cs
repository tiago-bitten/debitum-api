using Application.Customers.Ports;
using Application.Shared.Ports;
using Infra.Postgres.Customers.Repositories;
using Infra.Postgres.Shared.Events;
using Infra.Postgres.Shared.Options;
using Infra.Postgres.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraPostgres(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCustomOptions(configuration);
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddCustomOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<PostgresOptions>(
            configuration.GetSection("Database:Postgres"));

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("Database:Postgres:ConnectionString").Value
            ?? throw new InvalidOperationException("PostgreSQL connection string is not configured");

        services.AddDbContext<DebitumDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(DebitumDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });

            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IEventsDispatcher, EventsDispatcher>();

        return services;
    }
}
