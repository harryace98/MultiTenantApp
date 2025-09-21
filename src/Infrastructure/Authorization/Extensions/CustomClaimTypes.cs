using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Authorization.Extensions
{
    public static class CustomClaimTypes
    {
        public const string TENANT_ID = "tenantid";
        public const string PERMISSIONS = "permissions";
    }
}
