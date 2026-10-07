using System.Security.Claims;

namespace TablexAPI.Extensions;

public static class UserClaimsExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
        => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
