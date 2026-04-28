using MaisonBean.Application.Auth.Commands;
namespace MaisonBean.Application.Interfaces;

public interface IAuthService
{
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<RegisterResult> RegisterAsync(RegisterCommand cmd, CancellationToken ct = default);
    Task<LoginResult> LoginAsync(LoginCommand cmd, CancellationToken ct = default);
}