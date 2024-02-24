using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DTO;
using MovieAPI.Models;
using MovieAPI.Models.Domain;
using MovieAPI.Repo;
using MovieAPI.Services.AuthenticationService.Handlers;
using MovieAPI.Services.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace YtMovieApis.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private CheckUsername _checkUsername;

        public AuthorizationController(DatabaseContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;

            SetCheckers();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            StatusDto status = new StatusDto();

            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "please pass all the valid fields";
                return Ok(status);
            }

            status = await _checkUsername.HandleAsync(model);

            return Ok(status);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(model.Username);
            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);

            if (user is null || !isPasswordCorrect) return Ok(new LoginResponseDto
            {
                StatusCode = 0,
                Message = "Invalid Username or Password",
                Token = "",
                Expiration = null
            });

            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            List<Claim> claims = GetClaims(user, userRoles);
            TokenResponseDto token = _tokenService.GetToken(claims);
            string refreshToken = _tokenService.GetRefreshToken();
            TokenInfo? tokenInfo = _context.TokenInfo.FirstOrDefault(a => a.Usename == user.UserName);

            if (tokenInfo == null)
            {
                var info = new TokenInfo
                {
                    Usename = user.UserName,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = DateTime.Now.AddDays(1)
                };
                _context.TokenInfo.Add(info);
            }
            else
            {
                tokenInfo.RefreshToken = refreshToken;
                tokenInfo.RefreshTokenExpiry = DateTime.Now.AddDays(1);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new LoginResponseDto
            {
                Name = user.Name,
                Username = user.UserName,
                Token = token.TokenString,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
                StatusCode = 1,
                Message = "Logged in"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Registration([FromBody]RegistrationDto model)
        {
            var status = new StatusDto();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
            }

            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists!=null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return Ok(status);
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };

            var result= await _userManager.CreateAsync(user, model.Password); 
            if(!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return Ok(status);
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";
            return Ok(status);
        }

       [HttpPost]
        public async Task<IActionResult> RegistrationAdmin([FromBody] RegistrationDto model)
        {
            var status = new StatusDto();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
            }

            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return Ok(status);
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return Ok(status);
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";
            return Ok(status);
        }

        private void SetCheckers()
        {
            _checkUsername = new CheckUsername() { _userManager = _userManager };
            var checkPassword = new CheckPassword() { _userManager = _userManager };
            var changePassword = new ChangePassword() { _userManager = _userManager };
            _checkUsername
                .SetNext(checkPassword)
                .SetNext(changePassword);
        }

        private static List<Claim> GetClaims(ApplicationUser user, IList<string> userRoles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            authClaims.AddRange(userRoles.Select(ur => new Claim(ClaimTypes.Role, ur)));
            return authClaims;
        }
    }
}
