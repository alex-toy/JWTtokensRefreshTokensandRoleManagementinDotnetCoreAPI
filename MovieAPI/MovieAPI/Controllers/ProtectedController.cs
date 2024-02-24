using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DTO;

namespace YtMovieApis.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    [Authorize]
    public class ProtectedController : ControllerBase
    {
        public IActionResult GetData()
        {
            var status = new StatusDto();
            status.StatusCode = 1;
            status.Message = "Data from protected controller";
            return Ok(status);
        }
    }
}
