using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthQuest.API.Services;
using HealthQuest.Core.Entities;
using HealthQuest.Infrastructure.Data;

namespace HealthQuest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly HealthQuestDbContext _context;
    private readonly IAuthService _authService;

    public AuthController(HealthQuestDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already registered" });
        }

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _authService.HashPassword(request.Password),
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // Create patient or provider profile
        if (request.Role == UserRole.Patient)
        {
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = request.Name,
                DateOfBirth = request.DateOfBirth ?? DateTime.UtcNow.AddYears(-30),
                Condition = request.Condition ?? "General",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Patients.Add(patient);
        }
        else if (request.Role == UserRole.Provider)
        {
            var provider = new Provider
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = request.Name,
                Specialty = request.Specialty ?? "General Practitioner",
                HospitalName = request.HospitalName ?? "",
                CreatedAt = DateTime.UtcNow
            };

            _context.Providers.Add(provider);
        }

        await _context.SaveChangesAsync();

        var token = _authService.GenerateJwtToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = _authService.GenerateJwtToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        });
    }
}

public record RegisterRequest(
    string Email,
    string Password,
    string Name,
    UserRole Role,
    DateTime? DateOfBirth = null,
    string? Condition = null,
    string? Specialty = null,
    string? HospitalName = null
);

public record LoginRequest(string Email, string Password);

public record AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
