using app.Models.Register;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Login([FromBody] UserRegistrationRequest request)
    {
        var result = await _userService.RegisterUserAsync(request);
        if (result.IsSuccess)
            return Ok(result);
        else
            return BadRequest(result);
    }
}