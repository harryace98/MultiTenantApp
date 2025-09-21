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
    public class Role: ITenantEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Relaciones
        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
        public string TenantId { get; set; }
        public Tenant Tenant { get; set; }
        }
}
