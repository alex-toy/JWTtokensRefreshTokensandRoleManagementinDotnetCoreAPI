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
        public IActionResult Refresh(RefreshTokenRequestDto tokenApiModel)
        {
            if (tokenApiModel is null) return BadRequest("Invalid client request");
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            string? username = principal.Identity.Name;
            TokenInfo? user = _dbContext.TokenInfo.SingleOrDefault(u => u.Usename == username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
                return BadRequest("Invalid client request");
            var newAccessToken = _tokenService.GetToken(principal.Claims);
            var newRefreshToken = _tokenService.GetRefreshToken();
            user.RefreshToken = newRefreshToken;
            _dbContext.SaveChanges();
            return Ok(new RefreshTokenRequestDto()
            {
                AccessToken = newAccessToken.TokenString,
                RefreshToken = newRefreshToken
            });
        }

        //revoken is use for removing token enntry
        [HttpPost, Authorize]
        public IActionResult Revoke()
        {
            try
            {
                var username = User.Identity.Name;
                var user = _dbContext.TokenInfo.SingleOrDefault(u => u.Usename == username);
                if (user is null)
                    return BadRequest();
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
