using GeoCidadao.EngagementServiceAPI.Contracts.QueueServices;
using GeoCidadao.Jobs;
using GeoCidadao.Models.Extensions;
using Quartz;

namespace GeoCidadao.EngagementServiceAPI.Jobs.QueueJobs
{
    public class PostDeletedQueueJob(ILogger<PostDeletedQueueJob> logger, IPostDeletedQueueService service) : BaseJob(logger)
    {
        private readonly IPostDeletedQueueService _service = service;

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
                OnError($"Houve um erro ao consumir as mensagens da fila de exclusão de post: {ex.GetFullMessage()}", context, ex);
            }
        }
    }
}