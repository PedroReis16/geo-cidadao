using Microsoft.Extensions.Logging;
using GeoCidadao.Jobs.Config;
using Quartz;
using Quartz.Listener;
using System.Collections.Concurrent;

namespace GeoCidadao.Jobs.Listeners
{
    public class JobChainListener(ILogger<JobChainListener> logger) : JobListenerSupport()
    {
        private readonly ConcurrentDictionary<JobKey, JobKey> _chainLinks = new ConcurrentDictionary<JobKey, JobKey>();
        private readonly ILogger<JobChainListener> _logger = logger;
        public override string Name { get; } = JobConstants.JobChainListener;

        /// <summary>
        /// Add a chain mapping - when the Job identified by the first key completes
        /// the job identified by the second key will be triggered.
        /// </summary>
        /// <param name="firstJob">a JobKey with the name and group of the first job</param>
        /// <param name="secondJob">a JobKey with the name and group of the follow-up job</param>
        public void AddJobChainLink(JobKey firstJob, JobKey secondJob)
        {
            if (firstJob == null || secondJob == null)
            {
                throw new ArgumentException("Key cannot be null!");
            }
            if (firstJob.Name == null || secondJob.Name == null)
            {
                throw new ArgumentException("Key cannot have a null name!");
            }

            if (!_chainLinks.ContainsKey(firstJob))
            {
                _ = _chainLinks.TryAdd(firstJob, secondJob);
            }
        }

        public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            if (jobException != null)
            {
                if (!jobException.RefireImmediately)
                {
                    await RemoveJobsFromScheduler(context.JobDetail.Key, context);
                    return;
                }

                if (context.RefireCount < 4)
                    _logger.LogInformation($"Job {context.JobDetail.Key.Name} - Retry {context.RefireCount + 1} of 5");
                else
                {
                    jobException.RefireImmediately = false;
                    await RemoveJobsFromScheduler(context.JobDetail.Key, context);
                }

                return;
            }

            _ = _chainLinks.TryGetValue(context.JobDetail.Key, out JobKey? sj);

            if (sj == null)
                return;

            try
            {
                IJobDetail? jobDetail = await context.Scheduler.GetJobDetail(sj);
                if (jobDetail != null && context.Result != null)
                {
                    jobDetail.JobDataMap.Add(JobConstants.PreviousResultJobDataKey, context.Result);
                    await context.Scheduler.TriggerJob(sj, jobDetail.JobDataMap, cancellationToken).ConfigureAwait(false);
                }
                else
                    await context.Scheduler.TriggerJob(sj, cancellationToken).ConfigureAwait(false);

                _ = _chainLinks.Remove(context.JobDetail.Key, out JobKey? value);
            }
            catch (SchedulerException se)
            {
                _logger.LogError(se, $"Error encountered during chaining to Job '{sj}'");
            }
        }

        private async Task RemoveJobsFromScheduler(JobKey jobKey, IJobExecutionContext context)
        {
            if (_chainLinks.TryGetValue(jobKey, out JobKey? sj))
                await RemoveJobsFromScheduler(sj, context);

            _ = await context.Scheduler.DeleteJob(jobKey);
        }
    }
}