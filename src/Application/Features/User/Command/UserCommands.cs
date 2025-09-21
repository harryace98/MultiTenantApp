using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.User.Command
{
    /// <summary>
    /// Command to create a new user.
    /// </summary>
    public sealed record UserCreateCommand
    {
        [Required]
        public string Idn { get; set; }

        [Required]
        public string Name { get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public int ProfileId { get; set; }

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Command to update a user.
    /// </summary>
    public sealed record UserUpdateCommand
    {
        public string Idn { get; set; }

        [Required]
        public string Name { get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        public string Email { get; set; }

        public int ProfileId { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public sealed record ChangePasswordCommand
    {
        [Required]
        public string OldPassword { get; init; }
        [Required]
        public string NewPassword { get; init; }
    }
}
