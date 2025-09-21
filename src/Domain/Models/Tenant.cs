using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Tenant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string TenantId { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
