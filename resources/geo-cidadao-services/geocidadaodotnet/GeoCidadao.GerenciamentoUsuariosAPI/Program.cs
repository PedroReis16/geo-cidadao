using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GeoCidadao.Caching.Extensions;
using GeoCidadao.Models.Middlewares;
using GeoCidadao.GerenciamentoUsuariosAPI.Config;
using System.Text.Json.Serialization;
using System.Reflection;
using GeoCidadao.Models.Config;
using Quartz;
using GeoCidadao.Jobs.Config;
using GeoCidadao.Jobs.Listeners;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.Jobs.QueueJobs;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Services.QueueServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Services;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Cache;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao;
using GeoCidadao.Cloud.Extensions;
using GeoCidadao.GerenciamentoUsuariosAPI.Services.CacheServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Services.ConnectionServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.ConnectionServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Middlewares;
using Microsoft.Extensions.Options;
using GeoCidadao.Database.Extensions;
using GeoCidadao.OAuth.Extensions;
using GeoCidadao.OAuth.Models;


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


builder.Services.Configure<KeycloakAdminOptions>(builder.Configuration.GetSection(AppSettingsProperties.Keycloak).GetSection(AppSettingsProperties.KeycloakAdmin)!);

// Middlewares
builder.Services.AddTransient<GlobalExceptionHandler>();
builder.Services.AddInMemoryCache(builder.Configuration);
builder.Services.AddResponseCaching();
builder.Services.AddTransient<HttpResponseCacheHandler>();

// Services
builder.Services.AddBucketServices(); // -> Injeta a dependencia dos serviços de Bucket 
builder.Services.AddTransient<IUserProfileService, UserProfileService>();
builder.Services.AddTransient<IUserPictureService, UserPictureService>();
builder.Services.AddTransient<IUserCacheService, UserCacheService>();

// DAOs
builder.Services.AddTransient<IUserProfileDao, UserProfileDao>();
builder.Services.AddTransient<IUserPictureDao, UserPictureDao>();

// Dao Cache
builder.Services.AddTransient<IUserPictureDaoCache, UserPictureDaoCache>();

// Queue Services
builder.Services.AddSingleton<INewUserQueueJobService, NewUserQueueJobService>();
builder.Services.AddSingleton<INotifyUserChangedService, NotifyUserChangedService>();

// Cache Services
builder.Services.AddSingleton<IKeycloakAdminCacheService, KeycloakAdminCacheService>();
builder.Services.AddSingleton<IUserPictureCacheService, UserPictureCacheService>();

// Connection Services
builder.Services.AddTransient<IKeycloakService, KeycloakService>();

// Keycloak
builder.Services.AddSingleton<IKeycloakTokenProvider, KeycloakTokenProvider>();
builder.Services.AddTransient<KeycloakAdminHandler>();


// Http Clients
builder.Services.AddHttpClient<IKeycloakTokenProvider, KeycloakTokenProvider>(AppSettingsProperties.TokenClient, (sp, httpClient) =>
{
    var admin = sp.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;
    httpClient.BaseAddress = new Uri($"{admin.BaseUrl}/realms/{admin.Realm}/protocol/openid-connect/token");
});

builder.Services.AddHttpClient<IKeycloakService, KeycloakService>(AppSettingsProperties.KeycloakClient, (sp, httpClient) =>
{
    var admin = sp.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;
    httpClient.BaseAddress = new Uri($"{admin.BaseUrl}/admin/realms/{admin.Realm}/");
})
.AddHttpMessageHandler<KeycloakAdminHandler>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ForwardingHandler>();

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

builder.Services.ConfigureOAuth(builder.Configuration.GetRequiredSection(AppSettingsProperties.OAuth).Get<OAuthConfiguration>()!);

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

    // Fila de processamento de novos usuários
    q.AddJob<NewUserQueueJob>(j =>
    {
        j.WithIdentity(nameof(NewUserQueueJob));
    });

    q.AddTrigger(t =>
    {
        t.ForJob(nameof(NewUserQueueJob));
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
    options.SwaggerEndpoint($"/{basePath}/swagger/v1/swagger.json", "GeoCidadao.GerenciamentoUsuariosAPI v1");
    options.RoutePrefix = $"{basePath}/swagger";
});

app.UseMiddleware<GlobalExceptionHandler>();

app.UseResponseCaching();
app.UseMiddleware<HttpResponseCacheHandler>();

app.UsePathBase($"/{basePath}");

app.UseAuthorization();

app.MapControllers();

app.Run();
