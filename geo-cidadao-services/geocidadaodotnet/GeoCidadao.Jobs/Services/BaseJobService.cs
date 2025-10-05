using Microsoft.Extensions.Logging;
using MobilePacs.Jobs.Config;
using MobilePacs.Jobs.Listeners;
using Quartz;
using Quartz.Impl.Matchers;

namespace MobilePacs.Jobs.Services
{
    public interface IBaseJobService
    {
        protected Task ScheduleJobAsync(IJobDetail job, ITrigger trigger);
        protected Task ScheduleJobsChainAsync(string chainName, ITrigger trigger, params IJobDetail[] jobs);
    }

    public abstract class BaseJobService : IBaseJobService
    {
        protected readonly ILogger<BaseJobService> Logger;
        protected readonly ISchedulerFactory SchedulerFactory;

        protected BaseJobService(ISchedulerFactory schedulerFactory, ILogger<BaseJobService> logger)
        {
            SchedulerFactory = schedulerFactory;
            Logger = logger;
        }

        public virtual async Task ScheduleJobAsync(IJobDetail job, ITrigger trigger)
        {
            IScheduler scheduler = await GetScheduler();
            _ = await scheduler.ScheduleJob(job, trigger);
        }

        public virtual async Task ScheduleJobsChainAsync(string chainName, ITrigger trigger, params IJobDetail[] jobs)
        {
            if (jobs.Length == 0)
                return;

            if (jobs.Length == 1)
                await ScheduleJobAsync(jobs[0], trigger);

            IScheduler scheduler = await GetScheduler();

            if (scheduler.ListenerManager.GetJobListener(chainName) is JobChainListener listener)
            {
                await Task.WhenAll(jobs.Select(j => scheduler.AddJob(j, true, true)));

                for (int i = 0; i < jobs.Length - 1; i++)
                {
                    IJobDetail firstJob = jobs[i];
                    IJobDetail secondJob = jobs[i + 1];
                    listener.AddJobChainLink(firstJob.Key, secondJob.Key);
                }
                await scheduler.TriggerJob(jobs[0].Key);
            }
        }

        protected async Task<IScheduler> GetScheduler()
        {
            IScheduler? scheduler = await SchedulerFactory.GetScheduler(JobConstants.SchedulerName);
            if (scheduler == null)
                throw new Exception($"{JobConstants.SchedulerName} não definido");
            else
                return scheduler;
        }
    }
}
