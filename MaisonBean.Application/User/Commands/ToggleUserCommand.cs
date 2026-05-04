//using MaisonBean.Domain.Entities;
//using MediatR;
//using Microsoft.AspNetCore.Identity;

//namespace MaisonBean.Application.Orders.Commands;

//public record ToggleUserCommand(int Id) : IRequest<Unit>;


//public class ToggleUserCommandHandler
//    : IRequestHandler<ToggleUserCommand, Unit>
//{
//    private readonly UserManager<AppUser> _userManager;

//    public ToggleUserCommandHandler(UserManager<AppUser> userManager)
//    {
//        _userManager = userManager;
//    }

//    public async Task<Unit> Handle(ToggleUserCommand request, CancellationToken ct)
//    {
//        var user = await _userManager.FindByIdAsync(request.Id.ToString());

//        if (user == null)
//            throw new KeyNotFoundException("User not found");

//        user.ToggleBlock();

//        await _userManager.UpdateAsync(user);

//        return Unit.Value;
//    }
//}



using MediatR;
using Microsoft.AspNetCore.Identity;
using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.User.Commands;

public record ToggleUserCommand(int Id) : IRequest<bool>;

public class ToggleUserCommandHandler
    : IRequestHandler<ToggleUserCommand, bool>
{
    private readonly UserManager<AppUser> _userManager;

    public ToggleUserCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(ToggleUserCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user == null)
            throw new ArgumentException("User not found");

        // 🔥 Toggle block status
        user.ToggleBlock();

        // 🔥 IMPORTANT: invalidate old tokens
        user.TokenVersion++;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new Exception("Failed to update user");

        // ✅ return current state
        return user.IsBlocked;
    }
}