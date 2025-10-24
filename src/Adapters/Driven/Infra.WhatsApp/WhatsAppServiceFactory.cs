using Application.Shared.Options;
using Application.Shared.Ports;
using Infra.WhatsApp.Evolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infra.WhatsApp;

internal sealed class WhatsAppServiceFactory(
    IServiceProvider serviceProvider,
    IOptions<WhatsAppOptions> options) : IWhatsAppServiceFactory
{
    private readonly WhatsAppOptions _options = options.Value;

    public IWhatsAppService Create(string? providerName = null)
    {
        var provider = providerName ?? _options.Provider;

        return provider.ToLowerInvariant() switch
        {
            "evolution" => serviceProvider.GetRequiredService<EvolutionWhatsAppService>(),

            _ => throw new InvalidOperationException(
                $"WhatsApp provider '{provider}' is not supported. " +
                "Available providers: Evolution")
        };
    }
}