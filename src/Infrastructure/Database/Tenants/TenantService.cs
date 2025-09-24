#nullable enable

using Application.Abstractions.Data;
using Microsoft.Extensions.Configuration;
using System;

namespace Infrastructure.Database.Tenants
{
    public class TenantService : ITenantService
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext? _context;
        public TenantService(IConfiguration configuration, IApplicationDbContext context)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public TenantService(IConfiguration configuration) => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        public string TenantId { get; private set; } = string.Empty;
        public string? ConnectionString { get; private set; }

        public string GetConnectionString()
        {
            return _context?.Tenants.Find(TenantId)?.ConnectionString ?? throw new Exception("Tenant connection string dont exist.");
        }
        public void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(ConnectionString));
        }

        public void SetIdentityTenant()
        {
            TenantId = _configuration["IdentityTenantId"] ?? throw new ArgumentNullException("IdentityTenantId configuration is null.");
            ConnectionString = _configuration.GetConnectionString("IdentityConnection") ?? throw new ArgumentNullException("IdentityConnection configuration is null.");
        }

        public void SetTenant(string tenantId)
        {
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
}
