using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GeoCidadao.Caching.Extensions;
using GeoCidadao.Models.Middlewares;
using GeoCidadao.SearchServiceAPI.Config;
using System.Text.Json.Serialization;
using System.Reflection;
using GeoCidadao.Models.Config;
using GeoCidadao.Database.Extensions;
using GeoCidadao.OAuth.Extensions;
using GeoCidadao.OAuth.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using GeoCidadao.SearchServiceAPI.Services;
using GeoCidadao.SearchServiceAPI.Services.Contracts;
using GeoCidadao.SearchServiceAPI.BackgroundServices;


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

// Configure Elasticsearch
var elasticsearchConfig = builder.Configuration.GetSection("Elasticsearch").Get<ElasticsearchConfiguration>();
if (elasticsearchConfig != null && !string.IsNullOrEmpty(elasticsearchConfig.Url))
{
    var settings = new ElasticsearchClientSettings(new Uri(elasticsearchConfig.Url))
        .DisableDirectStreaming()
        .RequestTimeout(TimeSpan.FromSeconds(30));

    var client = new ElasticsearchClient(settings);
    builder.Services.AddSingleton(client);
}

// Middlewares
builder.Services.AddTransient<GlobalExceptionHandler>();
builder.Services.AddInMemoryCache(builder.Configuration);
builder.Services.AddResponseCaching();
builder.Services.AddTransient<HttpResponseCacheHandler>();

// Services
builder.Services.AddScoped<IElasticsearchIndexService, ElasticsearchIndexService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IPostDataService, PostDataService>();
builder.Services.AddScoped<IUserDataService, UserDataService>();

// Background Services
builder.Services.AddHostedService<PostChangedSubscriberService>();
builder.Services.AddHostedService<UserChangedSubscriberService>();
builder.Services.AddHostedService<NewUserSubscriberService>();

// DAOs

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

builder.Services.ConfigureOAuth(builder.Configuration.GetRequiredSection("OAuth").Get<OAuthConfiguration>()!);

WebApplication app = builder.Build();

app.ConfigureRequestLogging();

// Initialize Elasticsearch indices
using (var scope = app.Services.CreateScope())
{
    var indexService = scope.ServiceProvider.GetService<IElasticsearchIndexService>();
    if (indexService != null)
    {
        try
        {
            await indexService.InitializeIndicesAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Failed to initialize Elasticsearch indices");
        }
    }
}

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
    options.SwaggerEndpoint($"/{basePath}/swagger/v1/swagger.json", "GeoCidadao.SearchServiceAPI v1");
    options.RoutePrefix = $"{basePath}/swagger";
});

app.UseMiddleware<GlobalExceptionHandler>();

app.UseResponseCaching();
app.UseMiddleware<HttpResponseCacheHandler>();

app.UsePathBase($"/{basePath}");

app.UseAuthorization();

app.MapControllers();

app.Run();
