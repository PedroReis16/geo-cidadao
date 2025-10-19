using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices;
using GeoCidadao.Jobs;
using GeoCidadao.Model.Extensions;
using Quartz;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Models.Jobs.QueueJobs
{
    public class NewUserQueueJob(ILogger<NewUserQueueJob> logger, INewUserQueueJobService service) : BaseJob(logger), IJob
    {
        private readonly INewUserQueueJobService _service = service;

        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _ = base.Execute(context);

                _service.ConsumeQueue();
            }
            catch (JobExecutionException) { throw; }
            catch (Exception ex)
            {
                OnError($"Não foi possível processar o job de novo usuário: {ex.GetFullMessage()}", context, ex);
            }
        }
    }
}