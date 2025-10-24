namespace Application.Shared.Ports;

public interface IWhatsAppService
{
    Task<bool> SendTextMessageAsync(
        string instanceName,
        string recipientNumber,
        string message,
        CancellationToken cancellationToken = default);

    Task<bool> SendMediaMessageAsync(
        string instanceName,
        string recipientNumber,
        string mediaUrl,
        string? caption,
        CancellationToken cancellationToken = default);

    Task<bool> CheckInstanceStatusAsync(
        string instanceName,
        CancellationToken cancellationToken = default);
}