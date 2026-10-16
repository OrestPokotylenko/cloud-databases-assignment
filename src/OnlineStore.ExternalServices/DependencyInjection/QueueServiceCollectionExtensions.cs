using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Application.Orders.Messages;
using OnlineStore.ExternalServices.Messaging;
using OnlineStore.ExternalServices.Options;

namespace OnlineStore.ExternalServices.DependencyInjection;

public static class QueueServiceCollectionExtensions
{
    public static IServiceCollection AddQueueServices(
        this IServiceCollection services)
    {
        services.AddOptions<QueueStorageOptions>()
            .BindConfiguration(QueueStorageOptions.SectionName)
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                $"{QueueStorageOptions.SectionName}:ConnectionString is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.CreateOrderQueueName),
                $"{QueueStorageOptions.SectionName}:CreateOrderQueueName is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.OrderUpdateQueueName),
                $"{QueueStorageOptions.SectionName}:OrderUpdateQueueName is required.")
            .ValidateOnStart();

        services.AddSingleton<IMessagePublisher<CreateOrderMessage>>(provider =>
        {
            var options = provider
                .GetRequiredService<IOptions<QueueStorageOptions>>()
                .Value;

            return new AzureQueueMessagePublisher<CreateOrderMessage>(
                CreateQueueClient(
                    options.ConnectionString,
                    options.CreateOrderQueueName));
        });

        services.AddSingleton<IMessagePublisher<OrderActionMessage>>(provider =>
        {
            var options = provider
                .GetRequiredService<IOptions<QueueStorageOptions>>()
                .Value;

            return new AzureQueueMessagePublisher<OrderActionMessage>(
                CreateQueueClient(
                    options.ConnectionString,
                    options.OrderUpdateQueueName));
        });

        return services;
    }

    private static QueueClient CreateQueueClient(
        string connectionString,
        string queueName) =>
        new(
            connectionString,
            queueName,
            new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });
}
