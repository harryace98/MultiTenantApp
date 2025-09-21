using Domain.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class UserRole : ITenantEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string TenantId { get; set; }

        public User User { get; set; } = null!; 
        public Role Role { get; set; } = null!;
        public Tenant Tenant { get; set; }
    }
}
