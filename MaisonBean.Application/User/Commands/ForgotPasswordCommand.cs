using MaisonBean.Application.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.User.Commands;
public class ForgotPasswordCommand : IRequest<bool>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{6,}$")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
}


public class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, bool>
{
    private readonly IUserService _userService;

    public ForgotPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        if (request.NewPassword != request.ConfirmPassword)
            throw new ArgumentException("Passwords do not match");

        return await _userService.ForgotPasswordAsync(
            request.Email,
            request.NewPassword);
    }
}