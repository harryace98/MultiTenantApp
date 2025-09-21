using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace MultiTenantApp.API.Middleware;

public class RequestContextLoggingMiddleware(RequestDelegate next)
{
    private const string CORRELATION_ID_HEADER_NAME = "Correlation-Id";

    public Task Invoke(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(context)))
        {
            return next.Invoke(context);
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CORRELATION_ID_HEADER_NAME,
            out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
