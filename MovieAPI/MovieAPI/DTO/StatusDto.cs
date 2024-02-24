using Microsoft.AspNetCore.Identity;

namespace MovieAPI.DTO
{
    public class StatusDto
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public IdentityResult Result { get; set; }
    }
}
