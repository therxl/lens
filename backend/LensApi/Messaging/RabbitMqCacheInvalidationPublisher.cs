using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LensApi.Messaging;

public sealed class RabbitMqCacheInvalidationPublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqCacheInvalidationPublisher> _logger;

    public RabbitMqCacheInvalidationPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqCacheInvalidationPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<bool> PublishAsync(CacheInvalidationMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, JsonOptions));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: _options.QueueName,
                basicProperties: properties,
                body: body);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish cache invalidation message for {Reason}", message.Reason);
            return Task.FromResult(false);
        }
    }

    private IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        return factory.CreateConnection();
    }
}