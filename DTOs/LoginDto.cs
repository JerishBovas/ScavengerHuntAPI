using System.ComponentModel.DataAnnotations;

namespace ScavengerHunt_API.DTOs
{
    public record struct LoginDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Password { get; set; }
    }
}
