using GeoCidadao.PostIndexerWorker.Contracts.QueueServices;
using GeoCidadao.PostIndexerWorker.Services.QueueServices;
using GeoCidadao.Models.Config;
using GeoCidadao.PostIndexerWorker.Services;
using GeoCidadao.PostIndexerWorker.Contracts;
using GeoCidadao.PostIndexerWorker.Models.Extensions;
using GeoCidadao.PostIndexerWorker;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<PostIndexerWorker>();

builder.Services.ConfigureServiceLogs();

//Services
builder.Services.AddSingleton<IElasticSearchService, ElasticSearchService>();

// Queue Services
builder.Services.AddSingleton<INewPostQueueService, NewPostQueueService>();
builder.Services.AddSingleton<IPostDeletedQueueService, PostDeletedQueueService>();


// Elastic Search Configuration
builder.Services.AddElasticSearchService();

var host = builder.Build();

await host.Services.InitializeElasticSearchAsync();

await host.RunAsync();