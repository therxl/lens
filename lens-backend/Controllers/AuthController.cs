using Microsoft.AspNetCore.Mvc;

namespace lens_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username == "user" && request.Password == "1234")
        {
            return Ok(new { success = true, userId = "user", mode = "user" });
        }
        return BadRequest(new { success = false, message = "Invalid credentials" });
    }

    [HttpPost("guest")]
    public IActionResult Guest()
    {
        return Ok(new { success = true, userId = "guest", mode = "guest" });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}