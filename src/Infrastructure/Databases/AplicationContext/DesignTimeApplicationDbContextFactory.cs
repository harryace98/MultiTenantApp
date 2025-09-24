using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Databases.AplicationContext
{
    /// <summary>
    /// Factory class for creating instances of ApplicationDbContext at design time.
    /// This is used by EF Core tools such as migrations when the runtime service provider is not available.
    /// </summary>
    public class DesignTimeApplicationDbContextFactory(ITenantService tenantProvider) : IDesignTimeDbContextFactory<ApplicationDbContext> {

    /// <summary>
    /// Creates a new instance of ApplicationDbContext.
    /// </summary>
    /// <param name="args">Command line arguments passed to the application.</param>
    /// <returns>A new instance of ApplicationDbContext configured for design-time use.</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
        {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=baseDatabase;Username=postgres;Password=StrongPass123");

        return new ApplicationDbContext(optionsBuilder.Options, tenantProvider);
        }
    }
}
