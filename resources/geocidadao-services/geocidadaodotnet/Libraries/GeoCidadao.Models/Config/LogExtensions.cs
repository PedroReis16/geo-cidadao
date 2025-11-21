using GeoCidadao.Models.Constants;
using GeoCidadao.Models.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace GeoCidadao.Models.Config
{
    public static class LogExtensions
    {
        public static WebApplicationBuilder ConfigureServiceLogs(this WebApplicationBuilder builder)
        {
            bool useSeriLog = builder.Configuration.GetValue<bool>("UseSerilog");

            if (useSeriLog)
            {
                builder.Host.UseSerilog((context, configuration) =>
                 {
                     bool IsProduction = builder.Environment.IsProduction();
                     LogEventLevel logMinimumLevel = IsProduction ? LogEventLevel.Information : LogEventLevel.Debug;


                     configuration
                         .WriteTo.Async(wt =>
                         {
                             wt.Console(
                                 outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj}{NewLine}{Exception}",
                                 restrictedToMinimumLevel: logMinimumLevel,
                                     theme: Serilog.Sinks.SystemConsole.Themes.SystemConsoleTheme.Colored
                                 );

                             if (!IsProduction)
                                 wt.File(
                                     formatter: new CompactJsonFormatter(),
                                     path: $"{builder.Configuration.GetValue<string>("LogsDir")}/{builder.Configuration.GetValue<string>("BasePath")}.json",
                                     restrictedToMinimumLevel: logMinimumLevel,
                                     rollingInterval: RollingInterval.Day,
                                     rollOnFileSizeLimit: true,
                                     retainedFileTimeLimit: IsProduction ? null : TimeSpan.FromDays(7),
                                     shared: true,
                                     flushToDiskInterval: TimeSpan.FromSeconds(5) //Tempo de atualização do arquivo de log
                                 );
                         })
                         .WriteTo.Logger(logger =>
                             logger
                                  .Filter.ByExcluding(e => e.Properties["SourceContext"].ToString().Contains("Serilog.AspNetCore.RequestLoggingMiddleware"))
                                  .Filter.ByIncludingOnly(e =>
                                     e.Properties.TryGetValue(LogConstants.RequestMethod, out var requestMethod) &&
                                     requestMethod is ScalarValue rm &&
                                     (rm.Value?.ToString() == "POST"
                                         || rm.Value?.ToString() == "PUT"
                                         || rm.Value?.ToString() == "DELETE"
                                         || rm.Value?.ToString() == "PATCH")
                                 )
                                 .WriteTo.MongoDB(
                                     databaseUrl: builder.Configuration.GetConnectionString("GeoLogsDb")!
                                     )
                         );
                 });
            }

            return builder;
        }

        public static WebApplication ConfigureRequestLogging(this WebApplication app)
        {
            bool useSeriLog = app.Configuration.GetValue<bool>("UseSerilog");
            if (useSeriLog)
            {
                app.UseSerilogRequestLogging(options =>
                {
                    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        string? requestUser = httpContext.GetUser();
                        if (!string.IsNullOrWhiteSpace(requestUser))
                            diagnosticContext.Set(LogConstants.User, requestUser);

                        string? remoteIpAddress = httpContext.GetClientIpAddress();
                        if (!string.IsNullOrWhiteSpace(remoteIpAddress))
                            diagnosticContext.Set(LogConstants.RemoteAddress, remoteIpAddress);

                        diagnosticContext.Set(LogConstants.RequestId, httpContext.GetRequestId());
                        diagnosticContext.Set(LogConstants.ConnectionId, httpContext.GetConnectionId());
                        diagnosticContext.Set(LogConstants.Scheme, httpContext.GetScheme());
                        diagnosticContext.Set(LogConstants.Host, httpContext.GetHost());
                        diagnosticContext.Set(LogConstants.RequestMethod, httpContext.Request.Method);

                        if (httpContext.Request.Headers.TryGetValue("Content-Type", out var contentType))
                            diagnosticContext.Set(LogConstants.ContentType, contentType.ToString());
                        diagnosticContext.Set(LogConstants.Timestamp, httpContext.GetTimestamp());
                    };
                });
            }
            return app;
        }
    }
}