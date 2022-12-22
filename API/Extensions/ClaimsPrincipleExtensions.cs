using System.Security.Claims;

namespace API.Extensions;
// este archivo es todo lo q se necesita p' tener la extension,
// NO hay q dar de alta nada en program.cs ni 
// nada por el estilo, si hace esta clase y ya se puede ocupar
public static class ClaimsPrincipleExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
