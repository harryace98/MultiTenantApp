using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public sealed record LoginRequestDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        public string Email { get; init; }
        [Required]
        public string Password { get; init; }
    }
}
