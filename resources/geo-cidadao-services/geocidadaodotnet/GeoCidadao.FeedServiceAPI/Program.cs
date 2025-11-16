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
using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Services;
using Polly;
using Polly.Extensions.Http;


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

// External Services Configuration
var externalServicesConfig = builder.Configuration.GetSection("ExternalServices").Get<ExternalServicesConfig>()
    ?? new ExternalServicesConfig();

// Polly retry policy for resilience
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

// HTTP Clients with Polly
builder.Services.AddHttpClient<IPostsApiClient, PostsApiClient>(client =>
{
    client.BaseAddress = new Uri(externalServicesConfig.PostsApiUrl);
    client.Timeout = TimeSpan.FromSeconds(externalServicesConfig.TimeoutSeconds);
})
.AddPolicyHandler(retryPolicy)
.AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>(client =>
{
    client.BaseAddress = new Uri(externalServicesConfig.UsersApiUrl);
    client.Timeout = TimeSpan.FromSeconds(externalServicesConfig.TimeoutSeconds);
})
.AddPolicyHandler(retryPolicy)
.AddPolicyHandler(circuitBreakerPolicy);

// Services
builder.Services.AddScoped<IFeedService, FeedService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
