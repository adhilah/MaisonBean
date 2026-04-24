using Microsoft.AspNetCore.Identity;

namespace MaisonBean.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "customer";
    public string UserStatus { get; set; } = "active";
}