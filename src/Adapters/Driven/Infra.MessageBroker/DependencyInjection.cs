using Application.Shared.Ports;
using Infra.MessageBroker.Configuration;
using Infra.MessageBroker.Connection;
using Infra.MessageBroker.Consumers;
using Infra.MessageBroker.Messages;
using Infra.MessageBroker.Publishers;
using Infra.MessageBroker.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.MessageBroker;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraMessageBroker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));

        // Connection
        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();

        // Setup
        services.AddSingleton<RabbitMqSetup>();

        // Publisher
        services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();

        // Message Handler Registry (Singleton to hold registrations)
        services.AddSingleton<IMessageHandlerRegistry, MessageHandlerRegistry>();

        // Message Dispatcher
        services.AddScoped<MessageDispatcher>();

        // Register Message Handlers
        RegisterMessageHandlers(services);

        // Consumer Hosted Service
        services.AddHostedService<RabbitMqConsumerHostedService>();

        return services;
    }

    private static void RegisterMessageHandlers(IServiceCollection services)
    {
        // Register handler implementation
        services.AddScoped<IMessageHandler<SendDebtReminderMessage>, SendDebtReminderMessageHandler>();

        // Register in the registry (map message type name to handler)
        using var serviceProvider = services.BuildServiceProvider();
        var registry = serviceProvider.GetRequiredService<IMessageHandlerRegistry>();

        registry.RegisterHandler<SendDebtReminderMessage>(nameof(SendDebtReminderMessage));

        // Para adicionar novos handlers, apenas adicione aqui:
        // services.AddScoped<IMessageHandler<NovaMessage>, NovaMessageHandler>();
        // registry.RegisterHandler<NovaMessage>(nameof(NovaMessage));
    }

    public static void InitializeRabbitMq(this IServiceProvider serviceProvider)
    {
        var setup = serviceProvider.GetRequiredService<RabbitMqSetup>();
        setup.Initialize();
    }
}
