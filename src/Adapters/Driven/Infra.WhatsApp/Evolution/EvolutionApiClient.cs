using System.Net.Http.Json;
using System.Text.Json;
using Application.Shared.Options;
using Infra.WhatsApp.Evolution.DTOs;
using Microsoft.Extensions.Options;

namespace Infra.WhatsApp.Evolution;

internal sealed class EvolutionApiClient
{
    private readonly HttpClient _httpClient;
    private readonly EvolutionApiOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public EvolutionApiClient(HttpClient httpClient, IOptions<WhatsAppOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value.Evolution;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<bool> SendTextMessageAsync(
        string instanceName,
        SendTextMessageRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"/message/sendText/{instanceName}",
                request,
                _jsonOptions,
                cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendMediaMessageAsync(
        string instanceName,
        SendMediaMessageRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"/message/sendMedia/{instanceName}",
                request,
                _jsonOptions,
                cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CheckInstanceStatusAsync(
        string instanceName,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/instance/connectionState/{instanceName}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var status = await response.Content.ReadFromJsonAsync<InstanceStatusResponse>(
                _jsonOptions,
                cancellationToken);

            return status?.State?.Equals("open", StringComparison.OrdinalIgnoreCase) ?? false;
        }
        catch
        {
            return false;
        }
    }
}