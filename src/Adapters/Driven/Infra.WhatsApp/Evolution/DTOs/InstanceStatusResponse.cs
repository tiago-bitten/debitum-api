using System.Text.Json.Serialization;

namespace Infra.WhatsApp.Evolution.DTOs;

internal sealed class InstanceStatusResponse
{
    [JsonPropertyName("instance")] public InstanceInfo? Instance { get; init; }

    [JsonPropertyName("state")] public string? State { get; init; }
}

internal sealed class InstanceInfo
{
    [JsonPropertyName("instanceName")] public string? InstanceName { get; init; }

    [JsonPropertyName("status")] public string? Status { get; init; }
}