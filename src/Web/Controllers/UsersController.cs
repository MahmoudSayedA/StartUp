using Application.Identity.Services;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    // add role to user
    [HttpPost("add-role")]
    [Authorize]
    public Task<IActionResult> AddRoleToUser(string role)
    {
        return Task.FromResult<IActionResult>(Ok());
    }
}
