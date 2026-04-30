using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Auth.Commands;

public class LoginCommand : IRequest<LoginResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? Message { get; set; }
    public UserDto? User { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct)
    {
        return await _authService.LoginAsync(request, ct);
    }
}