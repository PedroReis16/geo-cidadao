using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using GeoCidadao.Models.Constants;

namespace GeoCidadao.Models.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogInformation(this ILogger logger, string message, HttpContext? context, Dictionary<string, object>? additionalProperties = null)
        {
            if (context != null)
            {
                Dictionary<string, object[]> body = GetMessage(message, context, additionalProperties);

                logger.LogInformation(body.Keys.First(), body.Values.First());

                return;
            }
            logger.LogInformation(message);
        }
        public static void LogError(this ILogger logger, string message, HttpContext context, Dictionary<string, object>? additionalProperties = null)
        {
            if (context != null)
            {
                Dictionary<string, object[]> body = GetMessage(message, context, additionalProperties);

                logger.LogError(body.Keys.First(), body.Values.First());

                return;
            }
            logger.LogError(message);
        }
        public static void LogError(this ILogger logger, Exception? exception, string message, HttpContext? context, Dictionary<string, object>? additionalProperties = null)
        {
            if (context != null)
            {
                Dictionary<string, object[]> body = GetMessage(message, context, additionalProperties);

                logger.LogError(exception, body.Keys.First(), body.Values.First());

                return;
            }
            logger.LogError(exception, message);
        }
        public static void LogWarning(this ILogger logger, string message, HttpContext context, Dictionary<string, object>? additionalProperties = null)
        {
            if (context != null)
            {
                Dictionary<string, object[]> body = GetMessage(message, context, additionalProperties);

                logger.LogWarning(body.Keys.First(), body.Values.First());

                return;
            }
            logger.LogWarning(message);
        }
        public static void LogWarning(this ILogger logger, string message, Exception? exception, HttpContext? context, Dictionary<string, object>? additionalProperties = null)
        {
            if (context != null)
            {
                Dictionary<string, object[]> body = GetMessage(message, context, additionalProperties);

                logger.LogWarning(exception, body.Keys.First(), body.Values.First());

                return;
            }
            logger.LogWarning(exception, message);
        }
        public static void LogCritical(this ILogger logger, string message, HttpContext context, Dictionary<string, object>? additionalProperties = null)
        {
            if (context != null)
            {
                Dictionary<string, object[]> body = GetMessage(message, context, additionalProperties);

                logger.LogCritical(body.Keys.First(), body.Values.First());

                return;
            }
            logger.LogCritical(message);
        }
        public static void LogCritical(this ILogger logger, Exception? exception, string message, HttpContext? context, Dictionary<string, object>? additionalProperties = null)
        {
            if (context != null)
            {
                Dictionary<string, object[]> body = GetMessage(message, context, additionalProperties);

                logger.LogCritical(exception, body.Keys.First(), body.Values.First());

                return;
            }
            logger.LogCritical(exception, message);
        }
        public static void LogDebug(this ILogger logger, string message, HttpContext? context, Dictionary<string, object>? additionalProperties = null)
        {
            if (context != null)
            {
                Dictionary<string, object[]> body = GetMessage(message, context, additionalProperties);
                logger.LogDebug(body.Keys.First(), body.Values.First());
                return;
            }
            logger.LogDebug(message);
        }
        public static void LogTrace(this ILogger logger, string message, HttpContext? context, Dictionary<string, object>? additionalProperties = null)
        {
            if (context != null)
            {
                Dictionary<string, object[]> body = GetMessage(message, context, additionalProperties);
                logger.LogTrace(body.Keys.First(), body.Values.First());

                return;
            }
            logger.LogTrace(message);
        }

        private static Dictionary<string, object[]> GetMessage(string message, HttpContext context, Dictionary<string, object>? additionalProperties = null)
        {
            Dictionary<string, object> scopeProperties = GetScopes(context);

            if (additionalProperties != null)
            {
                foreach (var kv in additionalProperties)
                {
                    if (!scopeProperties.ContainsKey(kv.Key))
                    {
                        scopeProperties[kv.Key] = kv.Value;
                    }
                }
            }

            var logProperties = new List<KeyValuePair<string, object>>(scopeProperties.Select(kv =>
                new KeyValuePair<string, object>(kv.Key, kv.Value)));

            return new()
            {
                { message + " {" + string.Join("} {", scopeProperties.Keys) + "}", logProperties.Select(kv => kv.Value).ToArray()}
            };
        }

        private static Dictionary<string, object> GetScopes(HttpContext context)
        {
            string requestId = context.GetRequestId();
            string connectionId = context.GetConnectionId();
            string scheme = context.GetScheme();
            string host = context.GetHost();
            string timestamp = context.GetTimestamp();
            string requestMethod = context.GetRequestMethod();
            string? clientIpAddress = context.GetClientIpAddress();
            string? user = context.GetUser();

            Dictionary<string, object> scope = new()
            {
                [LogConstants.RequestId] = requestId,
                [LogConstants.ConnectionId] = connectionId,
                [LogConstants.Scheme] = scheme,
                [LogConstants.Host] = host,
                [LogConstants.RequestMethod] = requestMethod,
                [LogConstants.Timestamp] = timestamp,
            };
            if (!string.IsNullOrWhiteSpace(clientIpAddress))
                scope[LogConstants.RemoteAddress] = clientIpAddress;
            if (!string.IsNullOrWhiteSpace(user))
                scope[LogConstants.User] = user;

            return scope;
        }
    }
}