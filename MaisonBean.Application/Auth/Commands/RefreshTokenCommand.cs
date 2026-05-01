using MaisonBean.Application.Auth.Commands;
using MaisonBean.Application.Interfaces;
using MediatR;

public class RefreshTokenCommand : IRequest<LoginResult>
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}


public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginResult> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        return await _authService.RefreshTokenAsync(
            request.Token,
            request.RefreshToken
        );  
    }
}