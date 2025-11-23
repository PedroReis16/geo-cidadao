using GeoCidadao.RelevanceWorker.Contracts.QueueServices;
using GeoCidadao.RelevanceWorker.Services.QueueServices;
using GeoCidadao.Models.Config;
using GeoCidadao.RelevanceWorker.Services;
using GeoCidadao.RelevanceWorker.Contracts;
using GeoCidadao.RelevanceWorker.Models.Extensions;
using GeoCidadao.RelevanceWorker;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RelevanceWorker>();

builder.Services.ConfigureServiceLogs();

//Services
builder.Services.AddSingleton<IElasticSearchService, ElasticSearchService>();
builder.Services.AddSingleton<IInteractionService, InteractionService>();

// Queue Services
builder.Services.AddSingleton<IPostDeletedQueueService, PostDeletedQueueService>();
builder.Services.AddSingleton<IPostInteractQueueService, PostInteractQueueService>();


// Elastic Search Configuration
builder.Services.AddElasticSearchService();

var host = builder.Build();

await host.Services.InitializeElasticSearchAsync();

await host.RunAsync();