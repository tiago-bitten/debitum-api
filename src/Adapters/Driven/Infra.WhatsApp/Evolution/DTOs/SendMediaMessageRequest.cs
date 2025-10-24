using System.Text.Json.Serialization;

namespace Infra.WhatsApp.Evolution.DTOs;

internal sealed class SendMediaMessageRequest
{
    [JsonPropertyName("number")] public required string Number { get; init; }

    [JsonPropertyName("mediatype")] public string MediaType { get; init; } = "image";

    [JsonPropertyName("media")] public required string Media { get; init; }

    [JsonPropertyName("caption")] public string? Caption { get; init; }

    [JsonPropertyName("delay")] public int Delay { get; init; } = 1200;
}