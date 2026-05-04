using MaisonBean.Application.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.User.Commands;

public class ChangePasswordCommand : IRequest<bool>
{
    public int UserId { get; set; }

    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{6,}$")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IUserService _userService;

    public ChangePasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        if (request.NewPassword != request.ConfirmPassword)
            throw new ArgumentException("Passwords do not match");

        return await _userService.ChangePasswordAsync(
            request.UserId,
            request.CurrentPassword,
            request.NewPassword);
    }
}