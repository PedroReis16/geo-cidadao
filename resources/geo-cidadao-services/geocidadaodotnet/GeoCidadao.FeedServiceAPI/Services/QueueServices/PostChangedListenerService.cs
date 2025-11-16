using GeoCidadao.AMQP.Contracts;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.FeedServiceAPI.Contracts;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeoCidadao.FeedServiceAPI.Services.QueueServices
{
    /// <summary>
    /// Serviço que escuta mudanças em posts e invalida o cache do feed
    /// </summary>
    public class PostChangedListenerService : IHostedService
    {
        private readonly ISubscriberService _subscriberService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PostChangedListenerService> _logger;
        private const string ExchangeName = "posts";
        private const string QueueName = "feed-service-post-changed";
        private const string RoutingKey = "post.changed";

        public PostChangedListenerService(
            ISubscriberService subscriberService,
            IServiceProvider serviceProvider,
            ILogger<PostChangedListenerService> logger)
        {
            _subscriberService = subscriberService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando listener de mudanças em posts");

            _subscriberService.ConsumeQueue(
                ExchangeName,
                QueueName,
                RoutingKey,
                OnPostChangedMessage,
                dlqExchangeName: $"{ExchangeName}-dlq",
                dlqQueueName: $"{QueueName}-dlq",
                dlqRoutingKeyName: $"{RoutingKey}-dlq"
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parando listener de mudanças em posts");
            return Task.CompletedTask;
        }

        private async void OnPostChangedMessage(object? sender, BasicDeliverEventArgs args)
        {
            try
            {
                var messageBody = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer.Deserialize<PostChangedMessage>(messageBody);

                if (message == null)
                {
                    _logger.LogWarning("Mensagem de post changed vazia ou inválida");
                    return;
                }

                _logger.LogInformation("Post {PostId} foi alterado, invalidando cache", message.PostId);

                // Cria um scope para resolver o serviço de feed
                using var scope = _serviceProvider.CreateScope();
                var feedService = scope.ServiceProvider.GetRequiredService<IFeedService>();

                // Invalida o cache
                await feedService.InvalidateFeedCacheAsync();

                _logger.LogInformation("Cache invalidado com sucesso para post {PostId}", message.PostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem de post changed");
            }
        }
    }
}
