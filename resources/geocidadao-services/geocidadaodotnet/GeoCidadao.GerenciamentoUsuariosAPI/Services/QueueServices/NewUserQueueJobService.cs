using System.Text.Json;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.Models.Entities;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Extensions;
using RabbitMQ.Client.Events;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services.QueueServices
{
    internal class NewUserQueueJobService(ILogger<NewUserQueueJobService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory) : RabbitMQSubscriberService(logger, configuration), INewUserQueueJobService
    {
        private readonly IServiceScopeFactory _serviceProvider = scopeFactory;

        public void ConsumeQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.KEYCLOAK_EVENTS_TOPIC_EXCHANGE,
                queueName: QueueNames.USER_MANAGEMENT_NEW_USER_QUEUE_NAME,
                routingKey: RoutingKeyNames.NEW_USER_ROUTING_KEY,
                onNewMessage: OnNewUser,
                dlqExchangeName: ExchangeNames.DLQ_KEYCLOAK_EVENTS_TOPIC_EXCHANGE,
                dlqQueueName: QueueNames.DLQ_USER_MANAGEMENT_NEW_USER_QUEUE_NAME,
                dlqRoutingKeyName: RoutingKeyNames.DLQ_NEW_USER_ROUTING_KEY
            );
        }

        private void OnNewUser(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var newUserMessageBytes = e.Body.ToArray();
                NewUserMessage? message = JsonSerializer.Deserialize<NewUserMessage>(newUserMessageBytes, options: new() { PropertyNameCaseInsensitive = true });

                if (message != null)
                {
                    using IServiceScope scope = _serviceProvider.CreateScope();
                    IUserProfileDao? userDao = scope.ServiceProvider.GetRequiredService<IUserProfileDao>();
                    IUserInterestsDao? interestsDao = scope.ServiceProvider.GetRequiredService<IUserInterestsDao>();

                    if (userDao is not null)
                    {
                        UserProfile newUser = new()
                        {
                            Id = new Guid(message.Id),
                            Username = message.Username,
                            Email = message.Email,
                            FirstName = message.FirstName,
                            LastName = message.LastName,
                        };

                        _ = userDao.AddAsync(newUser);
                        Logger.LogInformation($"Novo usuário criado com sucesso: {newUser.Id} - {newUser.Username}.");
                    }
                }
                Channel?.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Não foi possível processar a mensagem de novo usuário recebida: {ex.GetFullMessage()}");
                Channel?.BasicNack(e.DeliveryTag, false, true);
            }
        }
    }
}