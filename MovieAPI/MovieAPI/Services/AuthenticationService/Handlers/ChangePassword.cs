using Microsoft.AspNetCore.Identity;
using MovieAPI.DTO;
using MovieAPI.Models.Domain;

namespace MovieAPI.Services.AuthenticationService.Handlers
{
    public class ChangePassword : BaseHandler<ChangePasswordDto>
    {
        public UserManager<ApplicationUser> _userManager { private get; set; }

        public override async Task<StatusDto> HandleAsync(ChangePasswordDto request)
        {
            StatusDto status = new StatusDto();
            ApplicationUser? user = await _userManager.FindByNameAsync(request.Username);

            IdentityResult result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            status.Result = result;
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "Failed to change password";
                return status;
            }
            else
            {
                status.StatusCode = 1;
                status.Message = "Password has changed successfully";
                status.Result = result;
            }

            return status;
        }
    }
}
