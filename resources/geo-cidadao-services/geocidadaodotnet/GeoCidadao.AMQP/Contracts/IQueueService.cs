namespace GeoCidadao.AMQP.Contracts;

public interface IQueueService : IDisposable
{
    void Initialize();
}
