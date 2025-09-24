#nullable enable

using Application.Abstractions.Data;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace Infrastructure.Services
{
    public class TenantService(IConfiguration configuration, IIdentityDbContext identityDbContext) : ITenantService
    {
        public string TenantId { get; private set; } = string.Empty;
        public string? ConnectionString { get; private set; }
        public void GetConnectionString()
        {
            if (string.IsNullOrEmpty(TenantId))
            {
                throw new InvalidOperationException("TenantId is not set. Please set the TenantId before getting the connection string.");
            }
            ConnectionString = identityDbContext.Tenants
                .Find(TenantId)?.ConnectionString ?? throw new ArgumentNullException("Connection string for the tenant is null.");
        }

        public void SetIdentityTenant()
        {
            TenantId = configuration["IdentityTenantId"] ?? throw new ArgumentNullException("IdentityTenantId configuration is null.");
            ConnectionString = configuration.GetConnectionString("IdentityConnection") ?? throw new ArgumentNullException("IdentityConnection configuration is null.");
        }

        public void SetTenantId(string tenantId)
        {
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
}
