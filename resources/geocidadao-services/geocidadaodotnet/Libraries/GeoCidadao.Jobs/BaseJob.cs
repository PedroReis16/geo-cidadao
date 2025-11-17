using Microsoft.Extensions.Logging;
using GeoCidadao.Jobs.Config;
using GeoCidadao.Jobs.Exceptions;
using Quartz;
using GeoCidadao.Models.Extensions;

namespace GeoCidadao.Jobs
{
    public abstract class BaseJob(ILogger<BaseJob> logger) : IJob
    {
        protected readonly ILogger<BaseJob> Logger = logger;

        public virtual Task Execute(IJobExecutionContext context)
        {
            Logger.LogDebug($"{context.JobDetail.Key.Name} job iniciado");
            return Task.CompletedTask;
        }

        protected virtual void OnError(IJobExecutionContext context, Exception ex, bool refire = true)
        {
            DealException(ex);

            Logger.LogError(ex, $"Erro desconhecido durante a execução do job '{context.JobDetail.Key.Name}': {ex.GetFullMessage()}");

            if (!refire)
                throw new JobExecutionException(ex, false);

            if (context.RefireCount < 4)
                throw new JobExecutionException(ex, true);
        }

        protected virtual void OnError(string message, IJobExecutionContext context, Exception ex, bool refire = true)
        {
            DealException(ex);

            Logger.LogError(ex, message);
            if (!refire)
                throw new JobExecutionException(ex, false);

            if (context.RefireCount < 4)
                throw new JobExecutionException(ex, true);
        }

        private void DealException(Exception ex)
        {
            if (ex is JobDataKeyNotFoundException)
            {
                Logger.LogError(ex, ex.Message);
                throw new JobExecutionException(ex, false);
            }
            else if (ex is JobExecutionException)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        protected virtual object? GetPreviousResult(IJobExecutionContext context)
        {
            if (context.MergedJobDataMap.TryGetValue(JobConstants.PreviousResultJobDataKey, out object? result))
                return result;

            return null;
        }

        protected virtual async Task InterruptJob(IJobExecutionContext context)
        {
            await context.Scheduler.Interrupt(context.JobDetail.Key);

            Logger.LogInformation($"A execução do job {context.JobDetail.Key.Name} foi interrompida");
        }
    }
}
