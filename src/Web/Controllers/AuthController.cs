using Application.Identity.Commands;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(Login), null, null);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var res = await _mediator.Send(command);
        return Ok(res);
    }


    //[HttpPost("refresh")]
    //public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    //{
    //    var res = await _mediator.Send(command);
    //    return Ok(res);
    //}
}
