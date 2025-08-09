using app.Models.Register;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
    {
        var user = await _userService.RegisterUserAsync(request);
        if (user.IsSuccess)
        {
            return Ok(new { Message = "User registered successfully." });
        }
        return BadRequest(new { Error = user.ErrorMessage ?? "Registration failed." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result.IsAuthenticated)
            return Ok(new UserLoginResponse { JwtToken = result.JwtToken, IsAuthenticated = true });
        else
            return Unauthorized(new { Message = "Invalid credentials." });
    }
}