using Application.Abstractions.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Authentication;

namespace Infrastructure.Authorization
{
    internal sealed class PermissionProvider(IApplicationDbContext context, IMemoryCache cache): IPermissionProvider
    {
        private const string PERMISSION_CACHE_KEY = "user_permissions_";
        private const string ALL_PERMISSIONS_CACHE_KEY = "all_permissions";
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);

        public async Task<ImmutableHashSet<string>> GetUserPermissionsAsync(int userId, string tenantId)
        {
            string cacheKey = $"{PERMISSION_CACHE_KEY}{userId}_{tenantId}";

            if (cache.TryGetValue(cacheKey, out ImmutableHashSet<string> cachedPermissions))
            {
                return cachedPermissions;
            }

            var permissions = await context.Users
                .Where(u => u.Id == userId && u.TenantId == tenantId)
                .SelectMany(u => u.UserRoles)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => $"{rp.Permission.Action.ToLower()}:{rp.Permission.Screen.ToLower()}")
                .Distinct()
                .ToListAsync();

            var permissionSet = permissions.ToImmutableHashSet();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_cacheDuration)
                .RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    // Log cache eviction if needed
                });

            cache.Set(cacheKey, permissionSet, cacheOptions);

            return permissionSet;
        }

        public async Task<IEnumerable<string>> GetAllPermissionsAsync()
        {

            if (cache.TryGetValue(ALL_PERMISSIONS_CACHE_KEY, out IEnumerable<string> cachedPermissions))
            {
                return cachedPermissions;
            }

            var permissions = await context.Permissions
                .Select(p => $"{p.Action}:{p.Screen}")
                .Distinct()
                .ToListAsync();

            cache.Set(ALL_PERMISSIONS_CACHE_KEY, permissions, _cacheDuration);

            return permissions;
        }

        public void InvalidateUserPermissionCache(Guid userId, string tenantId)
        {
            string cacheKey = $"{PERMISSION_CACHE_KEY}{userId}_{tenantId}";
            cache.Remove(cacheKey);
        }

    }
}
