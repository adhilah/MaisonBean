using MaisonBean.Application.Interfaces;
using MediatR;
using System.Text.Json.Serialization;

namespace MaisonBean.Application.Auth.Commands;

public class LogoutCommand : IRequest<bool>
{
    [JsonIgnore]
    public int UserId { get; set; }
}


public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken ct)
    {
        return await _authService.LogoutAsync(request.UserId, ct);
    }
}