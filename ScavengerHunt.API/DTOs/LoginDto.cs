using System.ComponentModel.DataAnnotations;

namespace ScavengerHunt.API.DTOs
{
    public record struct LoginDto
    {
        [Required(ErrorMessage = "Email address is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; init; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8), MaxLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; init; }
    }
}
