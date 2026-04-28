namespace MaisonBean.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(int id, CancellationToken ct = default);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> ForgotPasswordAsync(string email, string newPassword);
}

public class UserProfileDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserStatus { get; set; } = string.Empty;
}