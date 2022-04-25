using Microsoft.Identity.Web;
using System.Security.Claims;

namespace Graphitie.Extensions;

public static class CoreExtensions
{
    public static string GetObjectIdValue(this ClaimsPrincipal claim)
    {
        var id = claim.GetObjectId();

        if (string.IsNullOrEmpty(id))
        {
            throw new UnauthorizedAccessException();
        }

        return id;
    }

    public static string GetUserPrincipalName(this HttpContext context)
    {
        var id = context.User.Identity?.Name;

        if (string.IsNullOrEmpty(id))
        {
            throw new UnauthorizedAccessException();
        }

        return id;
    }
}
