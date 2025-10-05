using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Contracts;
using RabbitMQ.Client;

namespace GeoCidadao.AMQP.Services;

public abstract class RabbitMQQueueService : IQueueService
{
    protected readonly ILogger<RabbitMQQueueService> Logger;
    protected readonly RabbitMQConfiguration Configuration;
    protected IConnection? Connection;
    protected IModel? Channel;

    public RabbitMQQueueService(ILogger<RabbitMQQueueService> logger, IConfiguration configuration)
    {
        Logger = logger;

        Configuration = configuration
            .GetRequiredSection("RabbitMQ")
            .Get<RabbitMQConfiguration>()!;

        Initialize();
    }

    public virtual void Initialize()
    {
        try
        {
            if (Connection is null)
            {
                Logger.LogDebug($"Inicializando conexão com o RabbitMQ");
                ConnectionFactory factory = new()
                {
                    HostName = Configuration.HostName,
                    UserName = Configuration.Username,
                    Password = Configuration.Password
                };
                Connection = factory.CreateConnection();
            }
    
            if (Channel is null)
            {
                Channel = Connection.CreateModel();
                Logger.LogDebug($"Inicializando canal com o RabbitMQ");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Erro durante a inicialização do RabbitMQ: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        Connection?.Dispose();
        Channel?.Dispose();
    }
}
