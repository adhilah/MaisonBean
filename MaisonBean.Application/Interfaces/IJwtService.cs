using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(AppUser user, IList<string> roles);
    string GenerateRefreshToken();
}