using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GeoCidadao.Caching.Extensions;
using GeoCidadao.Models.Middlewares;
using GeoCidadao.FeedServiceAPI.Config;
using System.Text.Json.Serialization;
using System.Reflection;
using GeoCidadao.Models.Config;
using GeoCidadao.OAuth.Extensions;
using GeoCidadao.OAuth.Models;
using GeoCidadao.FeedServiceAPI.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using GeoCidadao.FeedServiceAPI.Services.ConnectionServices;
using GeoCidadao.FeedServiceAPI.Contracts.ConnectionServices;
using GeoCidadao.FeedServiceAPI.Services.CacheServices;
using GeoCidadao.FeedServiceAPI.Contracts.CacheServices;
using GeoCidadao.FeedServiceAPI.Middlewares;
using Microsoft.Extensions.Options;
using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Models.Extensions;

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

// Elastic Search
builder.Services.AddElasticSearchService();


// Middlewares
builder.Services.AddTransient<GlobalExceptionHandler>();
builder.Services.AddInMemoryCache(builder.Configuration);
builder.Services.AddResponseCaching();
builder.Services.AddTransient<HttpResponseCacheHandler>();

// Services
builder.Services.AddTransient<IFeedService, FeedService>();

//Dao
builder.Services.AddTransient<IPostsDaoService, PostsDaoService>();

// Connection Services
builder.Services.AddSingleton<IUserManagementService, UserManagementService>();
builder.Services.AddSingleton<IPostEngagementService, PostEngagementService>();

// Cache Services
builder.Services.AddSingleton<IKeycloakAdminCacheService, KeycloakAdminCacheService>();
builder.Services.AddSingleton<IUserInterestsCacheService, UserInterestsCacheService>();
builder.Services.AddSingleton<IViewedPostsCacheService, ViewedPostsCacheService>();

// Keycloak
builder.Services.AddSingleton<IKeycloakTokenProvider, KeycloakTokenProvider>();
builder.Services.AddTransient<KeycloakAdminHandler>();

// HTTP Clients
builder.Services.AddHttpClient<IKeycloakTokenProvider, KeycloakTokenProvider>(AppSettingsProperties.TokenClient, (sp, httpClient) =>
{
    var admin = sp.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;
    httpClient.BaseAddress = new Uri($"{admin.BaseUrl}/realms/{admin.Realm}/protocol/openid-connect/token");
});

builder.Services.AddHttpClient<IUserManagementService, UserManagementService>(AppSettingsProperties.UserManagementClient, (sp, httpClient) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    httpClient.BaseAddress = new Uri(configuration.GetSection(AppSettingsProperties.ApiUrls).GetValue<string>(AppSettingsProperties.GerenciamentoUsuariosAPI)!);
})
.AddHttpMessageHandler<KeycloakAdminHandler>();

builder.Services.AddHttpClient<IPostEngagementService, PostEngagementService>(AppSettingsProperties.EngagementClient, (sp, httpClient) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    httpClient.BaseAddress = new Uri(configuration.GetSection(AppSettingsProperties.ApiUrls).GetValue<string>(AppSettingsProperties.EngagementServiceAPI)!);
})
.AddHttpMessageHandler<KeycloakAdminHandler>();


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
