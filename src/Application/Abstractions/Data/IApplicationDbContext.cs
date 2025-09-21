using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Application.Abstractions.Data
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Permission> Permissions { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<RolePermission> RolePermissions { get; set; }
        DbSet<Tenant> Tenants { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        int SaveChanges();
        DatabaseFacade Database { get; }
        EntityEntry Entry(object entity);
    }
}

