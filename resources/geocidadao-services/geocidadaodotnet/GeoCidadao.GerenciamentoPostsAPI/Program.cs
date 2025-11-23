using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GeoCidadao.Caching.Extensions;
using GeoCidadao.Models.Middlewares;
using GeoCidadao.GerenciamentoPostsAPI.Config;
using System.Text.Json.Serialization;
using System.Reflection;
using GeoCidadao.Models.Config;
using GeoCidadao.Database.Extensions;
using GeoCidadao.GerenciamentoPostsAPI.Services;
using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.Cloud.Extensions;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Database.EFDao;
using GeoCidadao.GerenciamentoPostsAPI.Services.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.OAuth.Extensions;
using GeoCidadao.OAuth.Models;
using GeoCidadao.OAuth.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.GerenciamentoPostsAPI.Middlewares;
using Quartz;
using GeoCidadao.Jobs.Config;
using GeoCidadao.Jobs.Listeners;
using GeoCidadao.GerenciamentoPostsAPI.Jobs.QueueJobs;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.ConnectionServices;
using GeoCidadao.GerenciamentoPostsAPI.Services.ConnectionServices;
using Polly;
using Polly.Extensions.Http;
using GeoCidadao.GerenciamentoPostsAPI.Services.CacheServices;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.CacheServices;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string basePath = builder.Configuration.GetValue<string>("BasePath") ?? "";

builder.ConfigureServiceLogs();

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.UsePostgreSql(builder.Configuration);


// Middlewares
builder.Services.AddTransient<GlobalExceptionHandler>();
builder.Services.AddInMemoryCache(builder.Configuration);
builder.Services.AddResponseCaching();
builder.Services.AddTransient<HttpResponseCacheHandler>();

// Services
builder.Services.AddBucketServices();
builder.Services.AddTransient<IPostService, PostService>();
builder.Services.AddTransient<IPostMediaService, PostMediaService>();
builder.Services.AddTransient<IMediaBucketService, MediaBucketService>();

// DAOs
builder.Services.AddTransient<IPostDao, PostDao>();
builder.Services.AddTransient<IPostMediaDao, PostMediaDao>();
builder.Services.AddTransient<IPostLocationDao, PostLocationDao>();

// Cache Services
builder.Services.AddTransient<IPostMediasCacheService, PostMediasCacheService>();

// Queue Services
builder.Services.AddSingleton<INotifyPostChangedService, NotifyPostChangedService>();
builder.Services.AddSingleton<IUserDeletedQueueService, UserDeletedQueueService>();

// Connection Services
builder.Services.AddHttpClient<INominatimService, NominatimService>();

// Fetchers (OAuth - Resource Fetchers)
builder.Services.AddScoped<IResourceFetcher<Post>, PostFetcher>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ForwardingHandler>();

// Http Clients
builder.Services.AddHttpClient<INominatimService, NominatimService>(AppSettingsProperties.NominatimClient, (sp, httpClient) =>
{
    IConfigurationSection apiUrlSection = builder.Configuration.GetSection(AppSettingsProperties.ApiUrls);
    httpClient.BaseAddress = new Uri(apiUrlSection.GetValue<string>(AppSettingsProperties.NominatimAPI)!);
})
   .AddPolicyHandler(GetRetryPolicy())
   .AddHttpMessageHandler<ForwardingHandler>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "OAuth 2.0 Access Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            []
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    option.DocumentFilter<TagDescriptionsDocumentFilter>();
    option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.ConfigureOAuth(builder.Configuration.GetRequiredSection("OAuth").Get<OAuthConfiguration>()!);

//Quartz Settings
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
    q.AddJob<UserDeletedQueueJob>(j =>
    {
        j.WithIdentity(nameof(UserDeletedQueueJob));
    });

    q.AddTrigger(t =>
    {
        t.ForJob(nameof(UserDeletedQueueJob));
        t.StartNow();
    });

});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});


WebApplication app = builder.Build();

app.ConfigureRequestLogging();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        var scheme = app.Environment.IsProduction() ? "https" : "http";
        swagger.Servers = [new OpenApiServer { Url = $"{scheme}://{httpReq.Host.Value}/{basePath}" }];
    });
    c.RouteTemplate = basePath + "/swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"/{basePath}/swagger/v1/swagger.json", "GeoCidadao.GerenciamentoPostsAPI v1");
    options.RoutePrefix = $"{basePath}/swagger";
});

app.UseMiddleware<GlobalExceptionHandler>();

app.UseResponseCaching();
app.UseMiddleware<HttpResponseCacheHandler>();

app.UsePathBase($"/{basePath}");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}