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
                options => !string.IsNullOrWhiteSpace(options.UpdateOrderStatusQueueName),
                $"{QueueStorageOptions.SectionName}:UpdateOrderStatusQueueName is required.")
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

        services.AddSingleton<IMessagePublisher<UpdateOrderStatusMessage>>(provider =>
        {
            var options = provider
                .GetRequiredService<IOptions<QueueStorageOptions>>()
                .Value;

            return new AzureQueueMessagePublisher<UpdateOrderStatusMessage>(
                CreateQueueClient(
                    options.ConnectionString,
                    options.UpdateOrderStatusQueueName));
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
