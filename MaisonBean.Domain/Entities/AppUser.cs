using Microsoft.AspNetCore.Identity;

namespace MaisonBean.Domain.Entities;

public class AppUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public int TokenVersion { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }

    public bool IsBlocked { get; private set; }

    public void ToggleBlock()
    {
        IsBlocked = !IsBlocked;
    }
}