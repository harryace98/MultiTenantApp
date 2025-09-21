using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class LoginResponseDTO
    {
        [Required]
        public required string Token { get; set; } = string.Empty;
        [Required]
        public required string Idn { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; }
        public string LastName { get; set; }    
    }
}
