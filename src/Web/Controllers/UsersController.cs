using Application.Common.Exceptions;

namespace Web.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    // add role to user
    [HttpPost("add-role")]
    //[Authorize]
    public Task<IActionResult> AddRoleToUser(string role)
    {
        List<ErrorDetail> errors = [new(role, "role should be lower case.")];

        throw new ValidationException("role should be lower case.", errors);
        //return Task.FromResult<IActionResult>(Ok());
    }
}
