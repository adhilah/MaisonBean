using MaisonBean.Application.Auth.Commands;
using MaisonBean.Application.Auth;
using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace MaisonBean.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwtService;

    public AuthService(UserManager<AppUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => await _userManager.FindByEmailAsync(email) != null;

    public async Task<RegisterResult> RegisterAsync(RegisterCommand cmd, CancellationToken ct = default)
    {
        var user = new AppUser
        {
            UserName = cmd.Email,
            Email = cmd.Email,
            FirstName = cmd.FirstName,
            LastName = cmd.LastName,
        };

        var result = await _userManager.CreateAsync(user, cmd.Password);

        if (!result.Succeeded)
            return new RegisterResult
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };

        await _userManager.AddToRoleAsync(user, "customer");

        return new RegisterResult
        {
            Success = true,
            Message = "Registration successful"
        };
    }

    public async Task<LoginResult> LoginAsync(LoginCommand cmd, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(cmd.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, cmd.Password))
            return new LoginResult { Success = false, Message = "Invalid email or password." };

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        //Generate Refresh Token
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return new LoginResult
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "customer"
            }
        };
    }

    public async Task<bool> LogoutAsync(int userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return false;

        user.TokenVersion++;

       
        user.RefreshToken = null;
        user.RefreshTokenExpiry = DateTime.MinValue;

        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<LoginResult> RefreshTokenAsync(string token, string refreshToken)
    {
        var user = _userManager.Users
            .FirstOrDefault(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return new LoginResult
            {
                Success = false,
                Message = "Invalid or expired refresh token"
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        var newToken = _jwtService.GenerateToken(user, roles);

        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return new LoginResult
        {
            Success = true,
            Token = newToken,
            RefreshToken = newRefreshToken
        };
    }
        private string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
