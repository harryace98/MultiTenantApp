#nullable enable
using Application.Abstractions.Authentication;
using Infrastructure.Authentication;
using Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(IPermissionProvider permissionProvider)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Reject unauthenticated users
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        // Get user and tenant IDs
        int userId = context.User.GetUserId();
        string? tenantId = context.User.GetTenantId();

        // Reject if tenant ID is missing
        if (string.IsNullOrEmpty(tenantId))
        {
            return;
        }

        // Get permissions filtered by tenant
        ImmutableHashSet<string> permissions = await permissionProvider.GetUserPermissionsAsync(userId, tenantId);

        if (permissions.Contains(requirement.Permission.ToLower()))
        {
            context.Succeed(requirement);
        }
    }
}
