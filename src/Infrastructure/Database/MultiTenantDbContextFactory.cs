using Infrastructure.Database.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Database;

public class MultiTenantDbContextFactory : IDbContextFactory<ApplicationDbContext>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public MultiTenantDbContextFactory(
        IServiceProvider serviceProvider,
        DbContextOptions<ApplicationDbContext> options)
    {
        _serviceProvider = serviceProvider;
        _options = options;
    }

    public ApplicationDbContext CreateDbContext()
    {
        // Create a scope to resolve scoped services
        using var scope = _serviceProvider.CreateScope();
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        return new ApplicationDbContext(_options, tenantService);
    }
}