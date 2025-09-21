using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Interfaces
{
    public interface ITenantEntity
    {
        string TenantId { get; set; }
        Tenant Tenant { get; set; }
    }
}
