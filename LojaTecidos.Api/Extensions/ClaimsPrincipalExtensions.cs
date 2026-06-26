using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LojaTecidos.Api.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static string? ObterUsuarioId(this ClaimsPrincipal usuario) =>
        usuario.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? usuario.FindFirstValue(ClaimTypes.NameIdentifier);
}
