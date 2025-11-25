using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GeoCidadao.Caching.Extensions;
using GeoCidadao.Models.Middlewares;
using GeoCidadao.EngagementServiceAPI.Config;
using System.Text.Json.Serialization;
using System.Reflection;
using GeoCidadao.Models.Config;
using GeoCidadao.Database.Extensions;
using GeoCidadao.OAuth.Extensions;
using GeoCidadao.OAuth.Models;
using GeoCidadao.EngagementServiceAPI.Contracts;
using GeoCidadao.EngagementServiceAPI.Services;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.EngagementServiceAPI.Database.EFDao;
using GeoCidadao.Models.Entities.EngagementServiceAPI;
using GeoCidadao.EngagementServiceAPI.Contracts.QueueServices;
using GeoCidadao.EngagementServiceAPI.Services.QueueServices;
using Quartz;
using GeoCidadao.Jobs.Config;
using GeoCidadao.Jobs.Listeners;
using GeoCidadao.EngagementServiceAPI.Jobs.QueueJobs;
using GeoCidadao.EngagementServiceAPI.Middlewares;
using GeoCidadao.EngagementServiceAPI.Contracts.CacheServices;
using GeoCidadao.EngagementServiceAPI.Services.CacheServices;
using GeoCidadao.EngagementServiceAPI.Contracts.ConnectionServices;
using GeoCidadao.EngagementServiceAPI.Services.ConnectionServices;
using Microsoft.Extensions.Options;
using GeoCidadao.EngagementServiceAPI.Database.CacheContracts;
using GeoCidadao.EngagementServiceAPI.Database.Cache;


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

builder.Services.Configure<KeycloakAdminOptions>(builder.Configuration.GetSection(AppSettingsProperties.Keycloak).GetSection(AppSettingsProperties.Admin)!);

builder.Services.UsePostgreSql(builder.Configuration);

// Middlewares
builder.Services.AddTransient<GlobalExceptionHandler>();
builder.Services.AddInMemoryCache(builder.Configuration);
builder.Services.AddResponseCaching();
builder.Services.AddTransient<HttpResponseCacheHandler>();

// Services
builder.Services.AddTransient<IPostInteractionService, PostInteractionService>();
builder.Services.AddTransient<IPostCommentsService, PostCommentsService>();

// DAOs
builder.Services.AddTransient<IPostLikesDao, PostLikesDao>();
builder.Services.AddTransient<IPostCommentsDao, PostCommentsDao>();
builder.Services.AddTransient<ICommentLikesDao, CommentLikesDao>();

// Queue Services
builder.Services.AddSingleton<INotifyPostInteraction, NotifyPostInteractionService>();
builder.Services.AddSingleton<IPostDeletedQueueService, PostDeletedQueueService>();
builder.Services.AddSingleton<IUserChangedQueueService, UserChangedQueueService>();

// Cache Services
builder.Services.AddSingleton<IKeycloakAdminCacheService, KeycloakAdminCacheService>();
builder.Services.AddSingleton<IUserCacheService, UserCacheService>();
builder.Services.AddSingleton<IPostCommentsDaoCache, PostCommentsDaoCache>();

//Connection Services
builder.Services.AddTransient<IUserManagementService, UserManagementService>();

//Keycloak
builder.Services.AddSingleton<IKeycloakTokenProvider, KeycloakTokenProvider>();
builder.Services.AddTransient<KeycloakAdminHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ForwardingHandler>();

//Http Clients
builder.Services.AddHttpClient<IKeycloakTokenProvider, KeycloakTokenProvider>(AppSettingsProperties.TokenClient, (sp, httpClient) =>
{
    var admin = sp.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;
    httpClient.BaseAddress = new Uri($"{admin.BaseUrl}/realms/{admin.Realm}/protocol/openid-connect/token");
});

builder.Services.AddHttpClient<IUserManagementService, UserManagementService>(AppSettingsProperties.UserManagementClient, (sp, httpClient) =>
{
    IConfigurationSection apiUrlSection = builder.Configuration.GetRequiredSection(AppSettingsProperties.ApiUrls);
    httpClient.BaseAddress = new Uri(apiUrlSection.GetValue<string>(AppSettingsProperties.GerenciamentoUsuariosAPI)!);
})
.AddHttpMessageHandler<KeycloakAdminHandler>();


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

    // Jobs para consumo da fila de exclusão dos posts
    q.AddJob<PostDeletedQueueJob>(j =>
    {
        j.WithIdentity(nameof(PostDeletedQueueJob));
    });

    q.AddTrigger(t =>
    {
        t.ForJob(nameof(PostDeletedQueueJob));
        t.StartNow();
    });

    // Jobs para consumo da fila de alterações nos perfis dos usuários
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
    options.SwaggerEndpoint($"/{basePath}/swagger/v1/swagger.json", "GeoCidadao.EngagementServiceAPI v1");
    options.RoutePrefix = $"{basePath}/swagger";
});

app.UseMiddleware<GlobalExceptionHandler>();

app.UseResponseCaching();
app.UseMiddleware<HttpResponseCacheHandler>();

app.UsePathBase($"/{basePath}");

app.UseAuthorization();

app.MapControllers();

app.Run();

internal class PostCommentLikesDao
{
}