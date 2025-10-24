using Application.Shared.Options;
using Application.Shared.Ports;
using Infra.WhatsApp.Evolution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.WhatsApp;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraWhatsApp(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCustomOptions(configuration);
        services.AddWhatsAppProviders(configuration);
        services.AddWhatsAppFactory();

        return services;
    }

    private static IServiceCollection AddCustomOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<WhatsAppOptions>(
            configuration.GetSection(WhatsAppOptions.SectionName));

        return services;
    }

    private static IServiceCollection AddWhatsAppProviders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var whatsAppOptions = configuration
            .GetSection(WhatsAppOptions.SectionName)
            .Get<WhatsAppOptions>();

        if (whatsAppOptions is null)
        {
            throw new InvalidOperationException("WhatsApp settings are not configured");
        }

        services.AddHttpClient<EvolutionApiClient>(client =>
        {
            client.BaseAddress = new Uri(whatsAppOptions.Evolution.BaseUrl);
            client.DefaultRequestHeaders.Add("apikey", whatsAppOptions.Evolution.ApiKey);
            client.Timeout = TimeSpan.FromSeconds(whatsAppOptions.Evolution.TimeoutSeconds);
        });
        services.AddScoped<EvolutionWhatsAppService>();

        return services;
    }

    private static IServiceCollection AddWhatsAppFactory(this IServiceCollection services)
    {
        services.AddScoped<IWhatsAppServiceFactory, WhatsAppServiceFactory>();

        return services;
    }
}