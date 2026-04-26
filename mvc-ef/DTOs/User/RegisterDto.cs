using System.ComponentModel.DataAnnotations;

namespace mvc_ef.DTOs.User
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{8,}$",
    ErrorMessage = "Password must contain uppercase, lowercase, and a number.")]
        public string Password { get; set; } = "";
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = "";
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = "";
    }
}
