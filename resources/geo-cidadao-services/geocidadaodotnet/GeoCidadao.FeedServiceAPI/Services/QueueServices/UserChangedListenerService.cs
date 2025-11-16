using GeoCidadao.AMQP.Contracts;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.FeedServiceAPI.Contracts;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeoCidadao.FeedServiceAPI.Services.QueueServices
{
    /// <summary>
    /// Serviço que escuta mudanças em usuários e invalida o cache do feed
    /// </summary>
    public class UserChangedListenerService : IHostedService
    {
        private readonly ISubscriberService _subscriberService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserChangedListenerService> _logger;
        private const string ExchangeName = "users";
        private const string QueueName = "feed-service-user-changed";
        private const string RoutingKey = "user.changed";

        public UserChangedListenerService(
            ISubscriberService subscriberService,
            IServiceProvider serviceProvider,
            ILogger<UserChangedListenerService> logger)
        {
            _subscriberService = subscriberService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando listener de mudanças em usuários");

            _subscriberService.ConsumeQueue(
                ExchangeName,
                QueueName,
                RoutingKey,
                OnUserChangedMessage,
                dlqExchangeName: $"{ExchangeName}-dlq",
                dlqQueueName: $"{QueueName}-dlq",
                dlqRoutingKeyName: $"{RoutingKey}-dlq"
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parando listener de mudanças em usuários");
            return Task.CompletedTask;
        }

        private async void OnUserChangedMessage(object? sender, BasicDeliverEventArgs args)
        {
            try
            {
                var messageBody = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer.Deserialize<UserChangedMessage>(messageBody);

                if (message == null)
                {
                    _logger.LogWarning("Mensagem de user changed vazia ou inválida");
                    return;
                }

                _logger.LogInformation("Usuário {UserId} foi alterado, invalidando cache", message.UserId);

                // Cria um scope para resolver o serviço de feed
                using var scope = _serviceProvider.CreateScope();
                var feedService = scope.ServiceProvider.GetRequiredService<IFeedService>();

                // Invalida o cache
                await feedService.InvalidateFeedCacheAsync();

                _logger.LogInformation("Cache invalidado com sucesso para usuário {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem de user changed");
            }
        }
    }
}
