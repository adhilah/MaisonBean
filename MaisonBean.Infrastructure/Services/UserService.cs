using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MaisonBean.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;

    public UserService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserProfileDto?> GetProfileAsync(int id, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return null;

        //Get roles from Identity
        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,

            
            Role = roles.FirstOrDefault() ?? "Customer",

           
            UserStatus = user.IsBlocked ? "Blocked" : "Active"
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        //Prevent blocked users
        if (user.IsBlocked)
            return false;

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> ForgotPasswordAsync(string email, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        //Prevent blocked users
        if (user.IsBlocked)
            return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded;
    }
    public async Task<List<UserProfileDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        // Get all users from Identity
        var users = _userManager.Users.ToList();

        var result = new List<UserProfileDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "Customer",
                UserStatus = user.IsBlocked ? "Blocked" : "Active"
            });
        }

        return result;
    }
}