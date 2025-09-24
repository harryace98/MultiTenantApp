using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Databases.AplicationContext;

public class ApplicationDbContextFactory(ITenantService tenantService,
    DbContextOptions<ApplicationDbContext> options) : IDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext()
    {
        // Create a scope to resolve scoped services
        return new ApplicationDbContext(options, tenantService);
    }
}