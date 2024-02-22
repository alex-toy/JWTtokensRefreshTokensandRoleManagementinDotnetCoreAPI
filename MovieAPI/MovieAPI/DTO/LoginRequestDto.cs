using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DTO
{
    public class LoginRequestDto
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
