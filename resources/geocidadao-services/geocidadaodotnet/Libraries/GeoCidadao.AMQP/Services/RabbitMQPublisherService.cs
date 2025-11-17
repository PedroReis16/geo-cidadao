using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GeoCidadao.AMQP.Contracts;
using RabbitMQ.Client;

namespace GeoCidadao.AMQP.Services;

public class RabbitMQPublisherService(ILogger<RabbitMQPublisherService> logger, IConfiguration configuration) : RabbitMQQueueService(logger, configuration), IPublisherService
{
    public void ConfigureExchange(
        string exchangeName,
        string exchangeType,
        bool durable = true,
        bool autoDelete = false,
        string? dlqExchangeName = null)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(dlqExchangeName))
            {
                Channel?.ExchangeDeclare(
                    exchange: dlqExchangeName,
                    type: exchangeType,
                    durable: durable,
                    autoDelete: autoDelete,
                    arguments: new Dictionary<string, object> { { "message-ttl", TimeSpan.FromDays(1).TotalMilliseconds } }
                );
            }

            Channel?.ExchangeDeclare(
                exchange: exchangeName,
                type: exchangeType,
                durable: durable,
                autoDelete: autoDelete,
                arguments: null
            );
            Logger.LogDebug($"Exchange {exchangeName} - {exchangeType} configurada");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Erro ao configurar Exchange no RabbitMQ: {ex.Message}");
            throw;
        }
    }

    public void PublishMessage(object message, string exchange, string routingKey)
    {
        try
        {
            Channel?.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: JsonSerializer.SerializeToUtf8Bytes(message)
            );
        }
        catch (Exception ex)
        {
            Logger.LogError($"Erro ao publicar mensagem no RabbitMQ: {ex.Message}");
            throw;
        }
    }
}
