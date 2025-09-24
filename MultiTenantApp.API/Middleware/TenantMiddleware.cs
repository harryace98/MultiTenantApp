using Infrastructure.Authorization.Extensions;
using Infrastructure.Database.Tenants;

namespace MultiTenantApp.API.Middleware
{
    public class TenantMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        private readonly List<string> _excludedEnpointByURL = ["/api/Login"];
        public async Task InvokeAsync(HttpContext context, ITenantService tenantProvider)
        {
            bool isThenRequestToExcludedEndpoint = _excludedEnpointByURL.Any(s => s.Equals(context.Request.Path.ToString(), StringComparison.OrdinalIgnoreCase));
            if (isThenRequestToExcludedEndpoint)
            {
                tenantProvider.SetIdentityTenant();
                await _next(context);
            }
            string tenantIdFromHeader = context.User.FindFirst(CustomClaimTypes.TENANT_ID)?.Value ?? "";
            string tenantIdFromToken = context.Request.Headers["X-TenantId"].FirstOrDefault() ?? "";

            bool tenantsAreEqual = string.Equals(tenantIdFromHeader, tenantIdFromToken, StringComparison.OrdinalIgnoreCase);
            if (tenantsAreEqual)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorizeds");
                return;
            }

            tenantProvider.SetTenant(tenantIdFromToken);

            await _next(context);
        }
    }

    public static class TenantMiddlewareExtension
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}
