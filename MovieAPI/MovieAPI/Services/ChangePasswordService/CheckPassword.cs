using Microsoft.AspNetCore.Identity;
using MovieAPI.DTO;
using MovieAPI.Models.Domain;
using MovieAPI.Services.ChangePasswordService.Handlers;

namespace MovieAPI.Services.ChangePasswordService
{
    public class CheckPassword : BaseHandler<ChangePasswordDto>
    {
        public UserManager<ApplicationUser> _userManager { private get; set; }

        public override async Task<StatusDto> HandleAsync(ChangePasswordDto request)
        {
            StatusDto status = new StatusDto();
            ApplicationUser? user = await _userManager.FindByNameAsync(request.Username);

            bool isCorrectPassword = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isCorrectPassword)
            {
                status.StatusCode = 0;
                status.Message = "invalid current password";
                return status;
            }

            return await Proceed(request);
        }
    }
}
