namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices
{
    public interface INotifyUserChangedService
    {
        void NotifyUserChanged(Guid userId);
        void NotifyUserDeleted(Guid userId);
        void NotifyUserPhotoChanged(Guid userId);
    }
}