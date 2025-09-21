#nullable enable

using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using Infrastructure.Authorization.Extensions;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return int.TryParse(userId, out int parsedUserId)
            ? parsedUserId
            : throw new ApplicationException("User id is unavailable");
    }

    public static string? GetEmail(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.Email)
            ?? principal?.FindFirstValue(JwtRegisteredClaimNames.Email);
    }

    public static string? GetRole(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.Role);
    }

    public static string? GetTenantId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(CustomClaimTypes.TENANT_ID);
    }

    public static List<string> GetPermissions(this ClaimsPrincipal? principal)
    {
        // If permissions are stored as a single claim with comma-separated values
        var permissionsClaim = principal?.FindFirstValue(CustomClaimTypes.PERMISSIONS);
        if (!string.IsNullOrEmpty(permissionsClaim))
        {
            return permissionsClaim.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        }

        // Or if each permission is a separate claim
        return principal?.FindAll(CustomClaimTypes.PERMISSIONS).Select(c => c.Value).ToList() ?? new List<string>();
    }
}
