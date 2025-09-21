using Domain.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RolePermission : ITenantEntity
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public string TenantId { get; set; }
        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
        public Tenant Tenant { get; set; }
        }
}
