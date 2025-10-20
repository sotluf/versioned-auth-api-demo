using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lab9.Models;
using Lab9.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Lab9.Controllers;

[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[ApiVersion("3.0")]
[Route("api/[controller]")]
public class AuthController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    // POST: api/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userService.AuthenticateAsync(request.Email, request.Password);
        if (user is null)
        {
            user = await _userService.GetByEmailAsync(request.Email);
            if (user is not null)
            {
                await _userService.UpdateAsync(user with
                {
                    FailedLoginAttempts = user.FailedLoginAttempts + 1
                });
            }
            return Unauthorized(new { Message = "Invalid credentials." });
        }

        var token = GenerateJwtToken(user);
        await _userService.UpdateAsync(user with
        {
            LastLoginAt = DateTimeOffset.UtcNow
        });

        return Ok(new { Token = token });
    }

    // POST: api/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        return await _userService.RegisterAsync(user)
            ? Ok(new { message = "User registered successfully." })
            : BadRequest(new { message = "User already exists." });
    }


    // Helper method to generate JWT token
    private string GenerateJwtToken(User user)
    {
        const string tokenKey = "KeyKeyKeyKeyKeyKeyKeyKeyKeyKeyKeyKey";

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(tokenKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, "User")
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

// DTO for login request
public class LoginRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}
