using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Auth;

public class RegisterCommand : IRequest<RegisterResult>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken ct)
    {
        var exists = await _authService.EmailExistsAsync(request.Email, ct);
        if (exists)
            return new RegisterResult { Success = false, Message = "Email already registered." };

        var result = await _authService.RegisterAsync(request, ct); // ✅ capture result

        if (!result.Success)
            return new RegisterResult { Success = false, Message = result.Message }; // ✅ bubble up failure

        return new RegisterResult { Success = true, Message = "Account created successfully." };
    }
}