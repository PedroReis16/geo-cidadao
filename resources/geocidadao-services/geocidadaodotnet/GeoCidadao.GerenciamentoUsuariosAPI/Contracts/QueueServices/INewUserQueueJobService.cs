namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices
{
    public interface INewUserQueueJobService
    {
        void ConsumeQueue();
    }
}