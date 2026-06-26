using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LojaTecidos.Infrastructure.Identity;

public sealed class JwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string Token, DateTime ExpiraEm) Gerar(ApplicationUser usuario, IReadOnlyList<string> papeis)
    {
        var expiraEm = DateTime.UtcNow.AddHours(_settings.ExpiracaoHoras);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id),
            new(JwtRegisteredClaimNames.Email, usuario.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, usuario.Id),
            new(ClaimTypes.Name, usuario.Nome)
        };

        claims.AddRange(papeis.Select(papel => new Claim("role", papel)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.ChaveSecreta));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
