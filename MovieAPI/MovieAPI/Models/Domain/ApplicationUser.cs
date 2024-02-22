using Microsoft.AspNetCore.Identity;

namespace MovieAPI.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}
