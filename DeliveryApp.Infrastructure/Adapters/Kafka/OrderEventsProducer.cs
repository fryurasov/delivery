using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using DeliveryApp.Core;
using DeliveryApp.Core.Domain.Models.Order.DomainEvents;
using DeliveryApp.Core.Ports;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Queues.Order.Events;

namespace DeliveryApp.Infrastructure.Adapters.Kafka;

/// <summary>
///     OrderEventsProducer для Kafka
/// </summary>
public sealed class OrderEventsProducer : IOrderEventsProducer
{
    private readonly ProducerConfig _config;
    private readonly string _topicName;

    public OrderEventsProducer(IOptions<Settings> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value.MessageBrokerHost))
            throw new ArgumentException(nameof(options.Value.MessageBrokerHost));
        if (string.IsNullOrWhiteSpace(options.Value.OrderEventsTopic))
            throw new ArgumentException(nameof(options.Value.OrderEventsTopic));

        _config = new ProducerConfig
        {
            BootstrapServers = options.Value.MessageBrokerHost
        };
        _topicName = options.Value.OrderEventsTopic;
    }

    public async Task PublishOrderAssignedAsync(OrderAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new OrderAssignedIntegrationEvent
        {
            OrderId = notification.OrderId.ToString()
        };

        await Produce(notification.EventId.ToString(), integrationEvent, notification, cancellationToken);
    }

    public async Task PublishOrderCompletedAsync(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new OrderCompletedIntegrationEvent
        {
            OrderId = notification.OrderId.ToString()
        };

        await Produce(notification.EventId.ToString(), integrationEvent, notification, cancellationToken);
    }

    private async Task Produce<TIntegrationEvent, TDomainEvent>(
        string key,
        TIntegrationEvent integrationEvent,
        TDomainEvent domainEvent,
        CancellationToken cancellationToken)
        where TIntegrationEvent : IMessage
    {
        var message = new Message<string, byte[]>
        {
            Key = key,
            Value = integrationEvent.ToByteArray(),
            Headers = new Headers
            {
                { "event-id", Encoding.UTF8.GetBytes(key) },
                { "event-type", Encoding.UTF8.GetBytes(domainEvent!.GetType().Name) },
                { "occurred-at", Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O")) },
                { "content-type", "application/x-protobuf"u8.ToArray() },
                { "debug-json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(domainEvent)) }
            }
        };

        using var producer = new ProducerBuilder<string, byte[]>(_config).Build();
        await producer.ProduceAsync(_topicName, message, cancellationToken);
    }
}
