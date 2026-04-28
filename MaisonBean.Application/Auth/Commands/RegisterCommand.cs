using MaisonBean.Application.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.Auth.Commands;

public class RegisterCommand : IRequest<RegisterResult>
{
    [Required(ErrorMessage = "First name is required")]
    [MinLength(2)]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Last name is required")]
    [MinLength(2)]
    public string LastName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.com$",
    ErrorMessage = "Email must end with .com")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(
    @"^(?=.*[A-Z])(?=.*\d).{6,}$",
    ErrorMessage = "Password must be at least 6 characters and include 1 uppercase, and 1 number"
)]
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

        var result = await _authService.RegisterAsync(request, ct);

        if (!result.Success)
            return new RegisterResult { Success = false, Message = result.Message };

        return new RegisterResult { Success = true, Message = "Account created successfully." };
    }
}