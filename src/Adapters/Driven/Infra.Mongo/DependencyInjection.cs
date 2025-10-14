using Application.Customers.Ports;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Infra.Mongo.Customers.Repositories;
using Infra.Mongo.Shared.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Infra.Mongo;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraMongo(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomOptions(configuration);
        services.AddDatabase();
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MongoDbOptions>()
            .Bind(configuration.GetSection("Database:Mongo"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var settings = MongoClientSettings.FromConnectionString(options.ConnectionString);
            
            return new MongoClient(settings);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return client.GetDatabase(options.DatabaseName);
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        return services;
    }
}