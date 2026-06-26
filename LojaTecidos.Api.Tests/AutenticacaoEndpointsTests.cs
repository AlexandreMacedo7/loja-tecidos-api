using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LojaTecidos.Application.Common.Dtos;

namespace LojaTecidos.Api.Tests;

public class AutenticacaoEndpointsTests : IClassFixture<LojaTecidosApiFactory>
{
    private readonly HttpClient _client;

    public AutenticacaoEndpointsTests(LojaTecidosApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ComCredenciaisGerente_DeveRetornarToken()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "gerente@emporiotecidos.com.br", senha = "Gerente@123" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var token = await response.Content.ReadFromJsonAsync<TokenAutenticacaoDto>();
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token.Token));
        Assert.Contains("Gerente", token.Papeis);
    }

    [Fact]
    public async Task Clientes_SemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/clientes");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Clientes_ComTokenGerente_DeveRetornar200()
    {
        var token = await ObterTokenGerenteAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/clientes");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AlterarPerfil_ComTokenGerente_DeveRetornar403()
    {
        var token = await ObterTokenGerenteAsync();

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/clientes/{Guid.NewGuid()}/perfil")
        {
            Content = JsonContent.Create(new { categoria = 1 })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<string> ObterTokenGerenteAsync()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "gerente@emporiotecidos.com.br", senha = "Gerente@123" });

        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenAutenticacaoDto>();
        return token!.Token;
    }
}
