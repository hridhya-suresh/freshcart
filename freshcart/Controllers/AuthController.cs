using freshcart.Data;
using freshcart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace freshcart.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;


    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var existingUser =
            _context.Users
            .FirstOrDefault(x => x.Email == dto.Email);

        if (existingUser != null)
        {
            return BadRequest("Email already exists");
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();


        return Ok(new
        {
            message = "User inserted successfully"
        });
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email == dto.Email);

        if (user == null)
        {
            return Unauthorized("Invalid Email");
        }

        bool validPassword =
            BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

        if (!validPassword)
        {
            return Unauthorized("Invalid Password");
        }

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            Token = token,
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.FirstName),
        new Claim(ClaimTypes.Email, user.Email)
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            ));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                Convert.ToDouble(
                    _configuration["Jwt:ExpiryMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}