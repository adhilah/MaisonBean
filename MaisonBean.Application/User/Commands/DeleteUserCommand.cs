using MaisonBean.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MaisonBean.Application.User.Commands;
public record DeleteUserCommand(int Id) : IRequest<Unit>;
public class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly UserManager<AppUser> _userManager;

    public DeleteUserCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user == null)
            throw new KeyNotFoundException("User not found");

        await _userManager.DeleteAsync(user);

        return Unit.Value;
    }
}
