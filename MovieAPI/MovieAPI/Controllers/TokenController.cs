using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DTO;
using MovieAPI.Models.Domain;
using MovieAPI.Services.Tokens;
using System.Security.Claims;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly Repo.DatabaseContext _dbContext;
        private readonly ITokenService _tokenService;

        public TokenController(Repo.DatabaseContext ctx, ITokenService service)
        {
            _dbContext = ctx;
            _tokenService = service;
        }

        [HttpPost]
        public IActionResult Refresh(RefreshTokenDto refreshToken)
        {
            if (refreshToken is null) return BadRequest("Invalid client request");

            ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken.AccessToken);
            string? username = principal.Identity.Name;
            TokenInfo? user = _dbContext.TokenInfo.SingleOrDefault(u => u.Usename == username);
            bool isWrongRefreshToken = user.RefreshToken != refreshToken.RefreshToken;
            bool isExpiredToken = user.RefreshTokenExpiry <= DateTime.Now;
            if (user is null || isWrongRefreshToken || isExpiredToken) return BadRequest("Invalid client request");
            TokenResponseDto newAccessToken = _tokenService.GetToken(principal.Claims);
            string newRefreshToken = _tokenService.GetRefreshToken();
            user.RefreshToken = newRefreshToken;
            _dbContext.SaveChanges();

            return Ok(new RefreshTokenDto()
            {
                AccessToken = newAccessToken.TokenString,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost, Authorize]
        public IActionResult Revoke()
        {
            try
            {
                string? username = User.Identity.Name;
                TokenInfo? user = _dbContext.TokenInfo.SingleOrDefault(u => u.Usename == username);
                if (user is null) return BadRequest();
                user.RefreshToken = null;
                _dbContext.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
