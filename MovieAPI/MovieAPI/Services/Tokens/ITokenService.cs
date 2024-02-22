using MovieAPI.DTO;
using System.Security.Claims;

namespace MovieAPI.Services.Tokens
{
    public interface ITokenService
    {
        TokenResponseDto GetToken(IEnumerable<Claim> claim);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
