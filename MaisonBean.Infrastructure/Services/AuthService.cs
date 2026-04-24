//using MaisonBean.Application.Auth;
//using MaisonBean.Application.Interfaces;
//using MaisonBean.Domain.Entities;
//using Microsoft.AspNetCore.Identity;

//namespace MaisonBean.Infrastructure.Services;

//public class AuthService : IAuthService
//{
//    private readonly UserManager<AppUser> _userManager;
//    private readonly IJwtService _jwtService;

//    public AuthService(UserManager<AppUser> userManager, IJwtService jwtService)
//    {
//        _userManager = userManager;
//        _jwtService = jwtService;
//    }

//    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
//        => await _userManager.FindByEmailAsync(email) != null;

//    public async Task<RegisterResult> RegisterAsync(RegisterCommand cmd, CancellationToken ct = default)
//    {
//        var user = new AppUser
//        {
//            UserName = cmd.Email,
//            Email = cmd.Email,
//            FirstName = cmd.FirstName,
//            LastName = cmd.LastName,
//        };

//        var result = await _userManager.CreateAsync(user, cmd.Password);

//        if (!result.Succeeded)
//            return new RegisterResult
//            {
//                Success = false,
//                Message = string.Join(", ", result.Errors.Select(e => e.Description))
//            };

//        await _userManager.AddToRoleAsync(user, "customer");

//        return new RegisterResult
//        {
//            Success = true,
//            Message = "Registration successful"
//        };
//    }

//    public async Task<LoginResult> LoginAsync(LoginCommand cmd, CancellationToken ct = default)
//    {
//        var user = await _userManager.FindByEmailAsync(cmd.Email);

//        if (user == null || !await _userManager.CheckPasswordAsync(user, cmd.Password))
//            return new LoginResult { Success = false, Message = "Invalid email or password." };

//        var roles = await _userManager.GetRolesAsync(user);
//        var token = _jwtService.GenerateToken(user, roles);

//        return new LoginResult
//        {
//            Success = true,
//            Token = token,
//            User = new UserDto
//            {
//                Id = user.Id,
//                FirstName = user.FirstName,
//                LastName = user.LastName,
//                Email = user.Email!,
//                Role = roles.FirstOrDefault() ?? "customer"
//            }
//        };
//    }
//}




using MaisonBean.Application.Auth;
using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Identity;

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

        return new LoginResult
        {
            Success = true,
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = user.Role
            }
        };
    }
}