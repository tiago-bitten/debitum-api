using System.Text.Json.Serialization;

namespace Infra.WhatsApp.Evolution.DTOs;

internal sealed class SendTextMessageRequest
{
    [JsonPropertyName("number")] public required string Number { get; init; }

    [JsonPropertyName("text")] public required string Text { get; init; }

    [JsonPropertyName("delay")] public int Delay { get; init; } = 1200;
}