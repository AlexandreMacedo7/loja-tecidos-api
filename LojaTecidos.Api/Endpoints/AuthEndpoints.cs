using System.Security.Claims;
using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Autenticacao;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Api.Extensions;

namespace LojaTecidos.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Autenticação");

        group.MapPost("/login", async (
            LoginBody body,
            IUseCase<LoginUsuarioRequest, TokenAutenticacaoDto?> useCase,
            CancellationToken cancellationToken) =>
        {
            var resultado = await useCase.ExecuteAsync(
                new LoginUsuarioRequest(body.Email, body.Senha),
                cancellationToken);

            return resultado is null
                ? Results.Unauthorized()
                : Results.Ok(resultado);
        })
        .AllowAnonymous();

        return app;
    }

    private sealed record LoginBody(string Email, string Senha);
}

internal static class ClaimsPrincipalExtensions
{
    public static string? ObterUsuarioId(this ClaimsPrincipal usuario) =>
        usuario.FindFirstValue(ClaimTypes.NameIdentifier);
}
