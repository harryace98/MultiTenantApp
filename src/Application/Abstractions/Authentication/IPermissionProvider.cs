using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Authentication
{
    public interface IPermissionProvider
    {
        public Task<ImmutableHashSet<string>> GetUserPermissionsAsync(int userId, string tenantId);
        public Task<IEnumerable<string>> GetAllPermissionsAsync();
        public void InvalidateUserPermissionCache(Guid userId, string tenantId);

    }
}
