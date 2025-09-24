#nullable enable

namespace Infrastructure.Database.Tenants
{
    public interface ITenantService
    {
        string TenantId { get; }
        string? ConnectionString { get; }
        void SetTenant(string tenantId);
        string GetConnectionString();
        void SetConnectionString(string connectionString);
        void SetIdentityTenant();

        //event TenantChangedEventHandler OnTenantChanged;
    }
}
