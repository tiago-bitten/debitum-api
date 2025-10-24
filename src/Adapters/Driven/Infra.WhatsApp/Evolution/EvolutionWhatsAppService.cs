using Application.Shared.Ports;
using Infra.WhatsApp.Evolution.DTOs;

namespace Infra.WhatsApp.Evolution;

internal sealed class EvolutionWhatsAppService(EvolutionApiClient client) : IWhatsAppService
{
    public async Task<bool> SendTextMessageAsync(
        string instanceName,
        string recipientNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        var request = new SendTextMessageRequest
        {
            Number = recipientNumber,
            Text = message
        };

        return await client.SendTextMessageAsync(instanceName, request, cancellationToken);
    }

    public async Task<bool> SendMediaMessageAsync(
        string instanceName,
        string recipientNumber,
        string mediaUrl,
        string? caption,
        CancellationToken cancellationToken = default)
    {
        var request = new SendMediaMessageRequest
        {
            Number = recipientNumber,
            Media = mediaUrl,
            Caption = caption
        };

        return await client.SendMediaMessageAsync(instanceName, request, cancellationToken);
    }

    public async Task<bool> CheckInstanceStatusAsync(
        string instanceName,
        CancellationToken cancellationToken = default)
    {
        return await client.CheckInstanceStatusAsync(instanceName, cancellationToken);
    }
}