using GeoCidadao.Jobs;
using GeoCidadao.Models.Extensions;
using GeoCidadao.PostIndexerWorker.Contracts.QueueServices;
using Quartz;

namespace GeoCidadao.PostIndexerWorker.Jobs
{
    public class PostDeletedQueueJob(ILogger<PostDeletedQueueJob> logger, IPostDeletedQueueService service) : BaseJob(logger), IJob
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
                OnError($"Ocorreu um erro ao processar a fila de posts deletados: {ex.GetFullMessage()}", context, ex);
            }
        }
    }
}