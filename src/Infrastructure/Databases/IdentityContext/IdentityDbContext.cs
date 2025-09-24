using Application.Abstractions.Data;
using Domain.Models;
using Infrastructure.Databases.AplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Databases.IdentityContext
{
    public class IdentityDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IIdentityDbContext
    {
        /// <summary>
        /// Gets or sets the Tenants entity set.
        /// </summary>
        public DbSet<Tenant> Tenants { get; set; }
        /// <summary>
        /// Gets or sets the Users entity set.
        /// </summary>
        public DbSet<User> Users { get; set; }

        public DbSet<UserTenants> UserTenants { get; set; } //relacion de tenants con usuario

    }
}
