using GeoCidadao.Jobs;
using GeoCidadao.Models.Extensions;
using GeoCidadao.PostIndexerWorker.Contracts.QueueServices;
using Quartz;

namespace GeoCidadao.PostIndexerWorker.Jobs
{
    public class PostChangedQueueJob(ILogger<PostChangedQueueJob> logger, IPostInterectedQueueService service) : BaseJob(logger), IJob
    {
        private readonly IPostInterectedQueueService _service = service;

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
                OnError($"Ocorreu um erro ao processar a fila de novos posts: {ex.GetFullMessage()}", context, ex);
            }
        }
    }
}