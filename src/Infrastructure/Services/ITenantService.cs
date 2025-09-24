#nullable enable

using Infrastructure;

namespace Infrastructure.Services
{
    public interface ITenantService
    {
        string TenantId { get; }
        string? ConnectionString { get; }
        void SetTenantId(string tenantId);
        void GetConnectionString();
        void SetIdentityTenant();

        //event TenantChangedEventHandler OnTenantChanged;
    }
}
