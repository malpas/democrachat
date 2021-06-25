using System.ComponentModel.DataAnnotations;

namespace Democrachat.Auth.Models
{
    public record Register
    {
        [Required]
        [MinLength(5, ErrorMessage = "Username must be >5 chars")]
        [MaxLength(12, ErrorMessage = "Username must be <12 chars")]
        [RegularExpression("[A-Za-z]+", ErrorMessage = "Username must have letters only")]
        public string Username { get; init; }
        
        [Required]
        [MinLength(10, ErrorMessage = "Password must be at least 10 chars")]
        public string Password { get; init; }
    };
}