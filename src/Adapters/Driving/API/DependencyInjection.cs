namespace API;

internal static class DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static IServiceCollection AddAPI(this IServiceCollection services)
    {
        services.AddCustomCors();
        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();

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
}