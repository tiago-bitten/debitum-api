namespace Application.Shared.Options;

public sealed class WhatsAppOptions
{
    public const string SectionName = "WhatsApp";

    public string Provider { get; set; } = "Evolution";
    public EvolutionApiOptions Evolution { get; set; } = new();
}

public sealed class EvolutionApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultInstance { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}