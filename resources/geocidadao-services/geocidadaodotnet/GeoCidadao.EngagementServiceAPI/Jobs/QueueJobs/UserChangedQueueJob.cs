using GeoCidadao.EngagementServiceAPI.Contracts.QueueServices;
using GeoCidadao.Jobs;
using GeoCidadao.Models.Extensions;
using Quartz;

namespace GeoCidadao.EngagementServiceAPI.Jobs.QueueJobs
{
    public class UserChangedQueueJob(ILogger<UserChangedQueueJob> logger, IUserChangedQueueService service) : BaseJob(logger)
    {
        private readonly IUserChangedQueueService _service = service;

        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await base.Execute(context);
                _service.ConsumeQueue();
                _service.ConsumeUserDeletedQueue();
            }
            catch (JobExecutionException) { throw; }
            catch (Exception ex)
            {
                OnError($"Houve um erro ao consumir as mensagens da fila de alterações no perfil de usuário: {ex.GetFullMessage()}", context, ex);
            }
        }
    }
}