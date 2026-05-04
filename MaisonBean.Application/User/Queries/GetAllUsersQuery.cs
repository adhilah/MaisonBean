using MaisonBean.Application.Interfaces;
using MediatR;
namespace MaisonBean.Application.User.Queries;
public class GetAllUsersQuery : IRequest<List<UserProfileDto>>
{
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserProfileDto>>
{
    private readonly IUserService _userService;

    public GetAllUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<List<UserProfileDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        return await _userService.GetAllUsersAsync(ct);
    }
}