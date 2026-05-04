using System.Security.Claims;
using MaisonBean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class BlockedUserMiddleware
{
    private readonly RequestDelegate _next;

    public BlockedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var userId = int.Parse(userIdClaim.Value);

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null && user.IsBlocked)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Your account is blocked"
                });

                return;
            }
        }

        await _next(context);
    }
}