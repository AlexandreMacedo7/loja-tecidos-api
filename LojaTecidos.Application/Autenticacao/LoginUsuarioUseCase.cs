using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Common.Dtos;

namespace LojaTecidos.Application.Autenticacao;

public sealed record LoginUsuarioRequest(string Email, string Senha);

public sealed class LoginUsuarioUseCase : IUseCase<LoginUsuarioRequest, TokenAutenticacaoDto?>
{
    private readonly IAutenticacaoService _autenticacaoService;

    public LoginUsuarioUseCase(IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    public async Task<TokenAutenticacaoDto?> ExecuteAsync(
        LoginUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _autenticacaoService.AutenticarAsync(
            request.Email,
            request.Senha,
            cancellationToken);

        return resultado is null
            ? null
            : new TokenAutenticacaoDto(
                resultado.Token,
                resultado.Email,
                resultado.Nome,
                resultado.Papeis,
                resultado.ExpiraEm);
    }
}
