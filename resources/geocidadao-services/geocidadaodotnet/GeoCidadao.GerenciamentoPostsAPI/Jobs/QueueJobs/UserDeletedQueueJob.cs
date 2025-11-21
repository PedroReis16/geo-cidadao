using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.Jobs;
using GeoCidadao.Models.Extensions;
using Quartz;

namespace GeoCidadao.GerenciamentoPostsAPI.Jobs.QueueJobs
{
    public class UserDeletedQueueJob(ILogger<UserDeletedQueueJob> logger, IUserDeletedQueueService service) : BaseJob(logger)
    {
        private readonly IUserDeletedQueueService _service = service;

        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await base.Execute(context);
                _service.ConsumeQueue();
            }
            catch (JobExecutionException) { throw; }
            catch (Exception ex)
            {
                OnError($"Houve um erro ao tentar consumir a fila de usuários removidos: {ex.GetFullMessage()}", context, ex);
            }
        }
    }
}