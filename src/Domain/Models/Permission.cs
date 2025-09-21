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
    public class Permission : ITenantEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Screen { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string TenantId { get; set; }

        // Relaciones
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
        public Tenant Tenant { get; set; }
        }
}
