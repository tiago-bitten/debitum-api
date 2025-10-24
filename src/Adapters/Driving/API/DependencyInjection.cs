using System.Text.Json.Serialization;

namespace API;

internal static class DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static IServiceCollection AddAPI(this IServiceCollection services)
    {
        services.AddCustomCors();
        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddJsonSerializerOptions();
        services.AddSwagger();

        return services;
    }

    private static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(x => x.AddDefaultPolicy(option =>
            option.AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials()
        ));

        return services;
    }

    private static IServiceCollection AddJsonSerializerOptions(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(x => { x.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Debitum API",
                Version = "v1",
                Description = "API for Debitum application"
            });
        });

        return services;
    }
}