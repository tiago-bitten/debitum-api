# WhatsApp Integration

This project integrates with WhatsApp using a **Ports & Adapters** architecture that supports multiple providers.

## Architecture

This implementation uses the **Strategy Pattern** with a **Factory** for runtime provider selection.

### Ports (Application Layer)
- **IWhatsAppService** - Domain abstraction for WhatsApp messaging (Strategy Interface)
- **IWhatsAppServiceFactory** - Factory to create WhatsApp service instances at runtime
- **WhatsAppOptions** - Generic settings that support multiple providers

### Adapters (Infrastructure Layer)
- **Infra.WhatsApp** - Infrastructure implementation
- **EvolutionWhatsAppService** - Evolution API adapter (current implementation)
- **WhatsAppServiceFactory** - Concrete factory that selects providers at runtime

## Current Provider: Evolution API

Evolution API is an open-source WhatsApp Web API that allows you to send and receive messages.

### Configuration

Settings are in `appsettings.json`:

```json
{
  "WhatsApp": {
    "Provider": "Evolution",
    "Evolution": {
      "BaseUrl": "http://localhost:8080",
      "ApiKey": "change-this-super-secret-key",
      "DefaultInstance": "debitum-instance",
      "TimeoutSeconds": 30
    }
  }
}
```

### Docker Setup

Evolution API runs in Docker Compose:

```bash
docker-compose up -d
```

Services:
- **postgres** - Database (port 5432)
- **evolution-api** - WhatsApp API (port 8080)
- **app** - Debitum API (port 5000)

### Evolution API Endpoints

Access Evolution API Manager:
- URL: http://localhost:8080/manager
- API Key: `change-this-super-secret-key`

### Usage in Code

Inject `IWhatsAppServiceFactory` to select provider at runtime:

```csharp
public class MyCommandHandler(IWhatsAppServiceFactory whatsAppFactory)
{
    public async Task HandleAsync(MyCommand command, CancellationToken ct)
    {
        // Use default provider from configuration
        var whatsAppService = whatsAppFactory.Create();

        // OR specify provider at runtime
        // var whatsAppService = whatsAppFactory.Create("Evolution");

        await whatsAppService.SendTextMessageAsync(
            instanceName: "debitum-instance",
            recipientNumber: "5511999999999",
            message: "Hello from Debitum!",
            cancellationToken: ct
        );
    }
}
```

**Benefits of Factory Pattern:**
- ✅ Runtime provider selection
- ✅ Different providers per customer/tenant
- ✅ Easy A/B testing between providers
- ✅ All providers registered, factory chooses the right one

### API Endpoints

#### Send Text Message
```http
POST /api/whatsapp/send-message
Content-Type: application/json

{
  "recipientNumber": "5511999999999",
  "message": "Hello World!",
  "instanceName": "debitum-instance", // optional, uses default if not provided
  "provider": "Evolution"              // optional, uses configured default if not provided
}
```

**Response:**
```json
{
  "success": true,
  "message": "Message sent successfully",
  "instanceName": "debitum-instance"
}
```

#### Check Instance Status
```http
GET /api/whatsapp/status/{instanceName}?provider=Evolution
```

**Response:**
```json
{
  "instanceName": "debitum-instance",
  "isConnected": true,
  "status": "Connected"
}
```

## Adding New Providers

To add a new WhatsApp provider (e.g., Twilio, WhatsApp Business API):

### 1. Update `WhatsAppOptions` in Application layer

Add new provider options:
```csharp
public sealed class WhatsAppOptions
{
    public string Provider { get; set; } = "Evolution";
    public EvolutionApiOptions Evolution { get; set; } = new();
    public TwilioOptions Twilio { get; set; } = new();  // New provider
}

public sealed class TwilioOptions
{
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
}
```

### 2. Create adapter in `Infra.WhatsApp`

Implement `IWhatsAppService`:
```csharp
internal sealed class TwilioWhatsAppService : IWhatsAppService
{
    // Implement using Twilio SDK
    public async Task<bool> SendTextMessageAsync(...)
    {
        // Twilio-specific implementation
    }
}
```

### 3. Update `DependencyInjection.cs`

Register the new provider:
```csharp
private static IServiceCollection AddWhatsAppProviders(...)
{
    // Evolution API
    services.AddScoped<EvolutionWhatsAppService>();

    // Twilio - NEW
    services.AddScoped<TwilioWhatsAppService>();

    return services;
}
```

### 4. Update `WhatsAppServiceFactory.cs`

Add new case to factory:
```csharp
public IWhatsAppService Create(string? providerName = null)
{
    var provider = providerName ?? _options.Provider;

    return provider.ToLowerInvariant() switch
    {
        "evolution" => serviceProvider.GetRequiredService<EvolutionWhatsAppService>(),
        "twilio" => serviceProvider.GetRequiredService<TwilioWhatsAppService>(),  // NEW
        _ => throw new InvalidOperationException($"Provider '{provider}' not supported")
    };
}
```

### 5. Configure in appsettings.json

```json
{
  "WhatsApp": {
    "Provider": "Twilio",
    "Twilio": {
      "AccountSid": "ACxxxxx",
      "AuthToken": "your_auth_token"
    }
  }
}
```

That's it! The factory will now resolve Twilio at runtime.

## Benefits of This Architecture

- ✅ **Strategy Pattern with Factory** - Runtime provider selection
- ✅ **Provider agnostic** - Easy to switch between providers
- ✅ **Multi-tenant support** - Different providers per customer
- ✅ **Testable** - Mock `IWhatsAppServiceFactory` in tests
- ✅ **Clean separation** - Domain doesn't depend on infrastructure
- ✅ **Extensible** - Add new providers without changing existing code
- ✅ **No startup coupling** - All providers registered, factory chooses at runtime
