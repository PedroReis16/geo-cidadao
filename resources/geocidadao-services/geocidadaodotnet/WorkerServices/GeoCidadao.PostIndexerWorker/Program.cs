using GeoCidadao.PostIndexerWorker.Contracts.QueueServices;
using GeoCidadao.PostIndexerWorker.Services.QueueServices;
using GeoCidadao.Models.Config;
using Quartz;
using GeoCidadao.Jobs.Config;
using GeoCidadao.Jobs.Listeners;
using GeoCidadao.PostIndexerWorker.Config;
using GeoCidadao.PostIndexerWorker.Jobs;
using GeoCidadao.PostIndexerWorker.Services;
using GeoCidadao.PostIndexerWorker.Contracts;
using GeoCidadao.PostIndexerWorker.Models.Extensions;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.ConfigureServiceLogs();

//Services
builder.Services.AddSingleton<IElasticSearchService, ElasticSearchService>();

// Queue Services
builder.Services.AddSingleton<INewPostQueueService, NewPostQueueService>();
builder.Services.AddSingleton<IPostInterectedQueueService, PostInteractedQueueService>();
builder.Services.AddSingleton<IPostDeletedQueueService, PostDeletedQueueService>();

builder.Services.Configure<QuartzOptions>(options =>
{
    options.Scheduling.IgnoreDuplicates = true;
    options.Scheduling.OverWriteExistingData = false;
});

builder.Services.AddQuartz(q =>
{
    q.SchedulerId = q.SchedulerName = JobConstants.SchedulerName;

    q.AddJobListener<JobChainListener>();

    q.UseSimpleTypeLoader();
    q.UseInMemoryStore();
    q.UseDefaultThreadPool(tp =>
    {
        int maxConcurrency = builder.Configuration
                      .GetRequiredSection(AppSettingsProperties.MaxConcurrency)
                      .Get<int>();

        tp.MaxConcurrency = maxConcurrency > 0 ? maxConcurrency : Environment.ProcessorCount;
    });

    // Fila para processar novos posts
    q.AddJob<NewPostQueueJob>(j =>
    {
        j.WithIdentity(nameof(NewPostQueueJob));
    });

    q.AddTrigger(t =>
    {
        t.ForJob(nameof(NewPostQueueJob));
        t.StartNow();
    });

    // Fila para processar posts alterados
    q.AddJob<PostChangedQueueJob>(j =>
    {
        j.WithIdentity(nameof(PostChangedQueueJob));
    });

    q.AddTrigger(t =>
    {
        t.ForJob(nameof(PostChangedQueueJob));
        t.StartNow();
    });

    // Fila para processar posts alterados
    q.AddJob<PostDeletedQueueJob>(j =>
    {
        j.WithIdentity(nameof(PostDeletedQueueJob));
    });

    q.AddTrigger(t =>
    {
        t.ForJob(nameof(PostDeletedQueueJob));
        t.StartNow();
    });
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

// Elastic Search Configuration
builder.Services.AddElasticSearchService();

var host = builder.Build();

await host.Services.InitializeElasticSearchAsync();

await host.RunAsync();