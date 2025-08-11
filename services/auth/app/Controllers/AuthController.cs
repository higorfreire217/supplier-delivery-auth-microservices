using app.Models.Login;
using app.Models.TokenValidation;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var result = await _authService.LoginUserAsync(request);
        if (result.IsAuthenticated)
            return Ok(result);
        else
            return Unauthorized(result);
    }

    [HttpPost("validate")]
    public IActionResult ValidateToken([FromBody] TokenValidationRequest request)
    {
        var result = _authService.ValidateJwtToken(request);

        if (result.IsValid)
            return Ok(result);
        else
            return Unauthorized(result);
    }
}