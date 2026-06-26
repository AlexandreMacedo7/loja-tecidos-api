using LojaTecidos.Application.Abstractions;
using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;

namespace LojaTecidos.Infrastructure.Identity;

public sealed class AutenticacaoService : IAutenticacaoService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AutenticacaoService(
        UserManager<ApplicationUser> userManager,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<TokenAutenticacaoResultado?> AutenticarAsync(
        string email,
        string senha,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByEmailAsync(email.Trim());
        if (usuario is null)
            return null;

        if (!await _userManager.CheckPasswordAsync(usuario, senha))
            return null;

        var papeis = await _userManager.GetRolesAsync(usuario);
        var papeisLista = (IReadOnlyList<string>)[.. papeis];
        var (token, expiraEm) = _jwtTokenGenerator.Gerar(usuario, papeisLista);

        return new TokenAutenticacaoResultado(
            token,
            usuario.Email ?? email,
            usuario.Nome,
            papeisLista,
            expiraEm);
    }
}
