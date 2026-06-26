using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialNet.Application.DTOs.Auth;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;

namespace SocialNet.Application.Services;

public class AuthService : IAuthService
{
    private readonly DbContext _db;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;

    public AuthService(DbContext db, IUnitOfWork uow, IConfiguration config)
    {
        _db = db;
        _uow = uow;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var exists = await _db.Set<User>()
            .AnyAsync(u => u.Email == request.Email || u.Username == request.Username, ct);

        if (exists)
            throw new ApplicationException("El email o nombre de usuario ya está registrado.");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            DisplayName = request.DisplayName
        };

        _db.Set<User>().Add(user);
        await _uow.SaveChangesAsync(ct);

        return GenerateToken(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _db.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct)
            ?? throw new ApplicationException("Credenciales inválidas.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new ApplicationException("Credenciales inválidas.");

        return GenerateToken(user);
    }

    private AuthResponse GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var expires = DateTime.UtcNow.AddHours(24);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new AuthResponse(
            user.Id,
            user.Username,
            new JwtSecurityTokenHandler().WriteToken(token),
            expires);
    }
}
