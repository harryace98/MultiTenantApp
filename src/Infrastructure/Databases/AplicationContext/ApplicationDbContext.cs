using Application.Abstractions.Data;
using Domain.Models;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Databases.AplicationContext
{
    /// <summary>
    /// Represents the main database context for the application, implementing Entity Framework Core DbContext
    /// and providing access to all entity sets and database operations.
    /// </summary>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantProvider) : DbContext(options), IApplicationDbContext
    {
        private readonly string _tenantId = tenantProvider.TenantId;
        private readonly string _connectionString = tenantProvider.ConnectionString;

        /// <summary>
        /// Gets or sets the Users entity set.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the Roles entity set.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets the Permissions entity set.
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Gets or sets the UserRoles entity set representing the many-to-many relationship between Users and Roles.
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Gets or sets the RolePermissions entity set representing the many-to-many relationship between Roles and Permissions.
        /// </summary>
        public DbSet<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Gets or sets the Tenants entity set.
        /// </summary>
        public DbSet<Tenant> Tenants { get; set; }
        public DbContextOptions<ApplicationDbContext> Options { get; } = options;

        /// <summary>
        /// Configures the database model and relationships between entities.
        /// </summary>
        /// <param name="modelBuilder">The model builder instance used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserRole composite key and relationships
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Configure RolePermission composite key and relationships
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // Global query filters for multi-tenancy
            modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.TenantId == _tenantId && u.IsActive);
            modelBuilder.Entity<Role>()
                .HasQueryFilter(r => r.TenantId == _tenantId);
            modelBuilder.Entity<Permission>()
                .HasQueryFilter(p => p.TenantId == _tenantId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
