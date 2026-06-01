using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RecommendationsApi.Messaging;

public sealed class RecommendationsCacheInvalidationWorker : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);
    private static readonly IReadOnlyCollection<string> DefaultCacheKeys =
        new[] { "recommendations:portrait", "recommendations:landscape", "recommendations:macro", "recommendations:sports" };

    private readonly RabbitMqOptions _options;
    private readonly IDistributedCache _cache;
    private readonly ILogger<RecommendationsCacheInvalidationWorker> _logger;

    public RecommendationsCacheInvalidationWorker(
        IOptions<RabbitMqOptions> options,
        IDistributedCache cache,
        ILogger<RecommendationsCacheInvalidationWorker> logger)
    {
        _options = options.Value;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
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

                _logger.LogInformation("RabbitMQ cache invalidation worker connected to queue {QueueName}", _options.QueueName);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = channel.BasicGet(_options.QueueName, autoAck: false);
                    if (result == null)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                        continue;
                    }

                    try
                    {
                        var payload = JsonSerializer.Deserialize<CacheInvalidationMessage>(result.Body.ToArray(), JsonOptions);
                        var keys = payload?.CacheKeys is { Count: > 0 } ? payload.CacheKeys : DefaultCacheKeys;

                        foreach (var key in keys)
                        {
                            await _cache.RemoveAsync(key, stoppingToken);
                        }

                        _logger.LogInformation(
                            "Invalidated {Count} cache keys from RabbitMQ message {Reason}",
                            keys.Count,
                            payload?.Reason ?? "unknown");

                        channel.BasicAck(result.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to process cache invalidation message");
                        channel.BasicNack(result.DeliveryTag, multiple: false, requeue: false);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RabbitMQ cache invalidation worker disconnected, retrying in {Delay}", RetryDelay);
                await Task.Delay(RetryDelay, stoppingToken);
            }
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