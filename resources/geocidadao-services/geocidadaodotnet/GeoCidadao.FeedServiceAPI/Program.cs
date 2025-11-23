using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GeoCidadao.Caching.Extensions;
using GeoCidadao.Models.Middlewares;
using GeoCidadao.FeedServiceAPI.Config;
using System.Text.Json.Serialization;
using System.Reflection;
using GeoCidadao.Models.Config;
using GeoCidadao.Database.Extensions;
using GeoCidadao.OAuth.Extensions;
using GeoCidadao.OAuth.Models;
using GeoCidadao.FeedServiceAPI.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using GeoCidadao.FeedServiceAPI.Contracts;
using StackExchange.Redis;

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

// Middlewares
builder.Services.AddTransient<GlobalExceptionHandler>();
builder.Services.AddInMemoryCache(builder.Configuration);
builder.Services.AddResponseCaching();
builder.Services.AddTransient<HttpResponseCacheHandler>();

// Services
builder.Services.AddTransient<FeedService>();
builder.Services.AddTransient<IUserInterestsService, UserInterestsService>();
builder.Services.AddTransient<IEngagementService, EngagementService>();
builder.Services.AddSingleton<ISeenPostsService, SeenPostsService>();

// Redis
// builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
// {
//     var configuration = sp.GetRequiredService<IConfiguration>();
//     var redisConnectionString = configuration.GetConnectionString("Redis")!;
//     return ConnectionMultiplexer.Connect(redisConnectionString);
// });

// HTTP Clients
builder.Services.AddHttpClient<IUserInterestsService, UserInterestsService>("UserInterestsClient", (sp, httpClient) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var url = configuration.GetValue<string>("Services:GerenciamentoUsuariosAPI") ?? "http://gerenciamento-usuarios-api:8080";
    httpClient.BaseAddress = new Uri(url); 
});

builder.Services.AddHttpClient<IEngagementService, EngagementService>("EngagementClient", (sp, httpClient) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var url = configuration.GetValue<string>("Services:EngagementServiceAPI") ?? "http://engagement-service-api:8080";
    httpClient.BaseAddress = new Uri(url);
});

// Elastic Search
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var settings = configuration.GetSection("ElasticSearch").Get<ElasticSearchSettings>();
    
    if (settings == null) throw new Exception("ElasticSearch settings not found");

    var clientSettings = new ElasticsearchClientSettings(new Uri(settings.Uri))
        .DefaultIndex(settings.DefaultIndex)
        .Authentication(new BasicAuthentication(settings.UserName, settings.Password));
    return new ElasticsearchClient(clientSettings);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ForwardingHandler>();

// Swagger
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
    options.SwaggerEndpoint($"/{basePath}/swagger/v1/swagger.json", "GeoCidadao.FeedServiceAPI v1");
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
