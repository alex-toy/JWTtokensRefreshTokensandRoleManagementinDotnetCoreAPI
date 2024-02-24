using Microsoft.AspNetCore.Identity;
using MovieAPI.DTO;
using MovieAPI.Models.Domain;
using MovieAPI.Services.ChangePasswordService.Handlers;

namespace MovieAPI.Services.ChangePasswordService
{
    public class CheckUsername : BaseHandler<ChangePasswordDto>
    {
        public UserManager<ApplicationUser> _userManager { private get; set; }

        public override async Task<StatusDto> HandleAsync(ChangePasswordDto request)
        {
            StatusDto status = new StatusDto();
            ApplicationUser? user = await _userManager.FindByNameAsync(request.Username);

            if (user is null)
            {
                status.StatusCode = 0;
                status.Message = "invalid username";
                return status;
            }

            return await Proceed(request);
        }
    }
}
