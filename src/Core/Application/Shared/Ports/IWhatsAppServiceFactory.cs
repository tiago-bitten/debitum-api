namespace Application.Shared.Ports;

public interface IWhatsAppServiceFactory
{
    IWhatsAppService Create(string? providerName = null);
}