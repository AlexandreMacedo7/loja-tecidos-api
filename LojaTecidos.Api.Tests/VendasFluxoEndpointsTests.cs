using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Api.Tests;

[Collection("Api")]
public class VendasFluxoEndpointsTests : IClassFixture<LojaTecidosApiFactory>
{
    private readonly HttpClient _client;

    public VendasFluxoEndpointsTests(LojaTecidosApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task VendaAvista_DevePersistirUsuarioIdDoToken()
    {
        var token = await ObterTokenGerenteAsync();

        var codigoProduto = $"INT-{Guid.NewGuid():N}"[..12].ToUpperInvariant();

        var produtoRequest = new HttpRequestMessage(HttpMethod.Post, "/api/produtos")
        {
            Content = JsonContent.Create(new
            {
                codigoInterno = codigoProduto,
                nome = "Tecido Fluxo",
                nomeFornecedor = "Fornecedor Teste",
                categoria = (int)CategoriaProduto.Tecido,
                unidadeMedida = (int)UnidadeMedida.Metro,
                precoUnitario = 30m,
                estoqueInicial = 50m
            })
        };
        produtoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var produtoResponse = await _client.SendAsync(produtoRequest);
        Assert.Equal(HttpStatusCode.Created, produtoResponse.StatusCode);

        var vendaRequest = new HttpRequestMessage(HttpMethod.Post, "/api/vendas/avista")
        {
            Content = JsonContent.Create(new
            {
                itens = new[] { new { codigoInternoProduto = codigoProduto, quantidade = 2m } }
            })
        };
        vendaRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var vendaResponse = await _client.SendAsync(vendaRequest);

        Assert.Equal(HttpStatusCode.Created, vendaResponse.StatusCode);

        var venda = await vendaResponse.Content.ReadFromJsonAsync<VendaDto>();
        Assert.NotNull(venda);
        Assert.False(string.IsNullOrWhiteSpace(venda.UsuarioId));

        var obterRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/vendas/{venda.CodigoVenda}");
        obterRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var obterResponse = await _client.SendAsync(obterRequest);

        Assert.Equal(HttpStatusCode.OK, obterResponse.StatusCode);
        var vendaConsultada = await obterResponse.Content.ReadFromJsonAsync<VendaDto>();
        Assert.Equal(venda.UsuarioId, vendaConsultada!.UsuarioId);
    }

    [Fact]
    public async Task VendaAvista_SemItens_DeveRetornar400()
    {
        var token = await ObterTokenGerenteAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/vendas/avista")
        {
            Content = JsonContent.Create(new { itens = Array.Empty<object>() })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
