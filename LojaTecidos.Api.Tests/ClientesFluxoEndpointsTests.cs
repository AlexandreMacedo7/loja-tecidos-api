using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Common.Paginacao;

namespace LojaTecidos.Api.Tests;

[Collection("Api")]
public class ClientesFluxoEndpointsTests : IClassFixture<LojaTecidosApiFactory>
{
    private readonly HttpClient _client;

    public ClientesFluxoEndpointsTests(LojaTecidosApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CadastrarEListarClientes_DeveRetornarClienteNaPaginacao()
    {
        var token = await ObterTokenGerenteAsync();

        var cadastro = new HttpRequestMessage(HttpMethod.Post, "/api/clientes")
        {
            Content = JsonContent.Create(new
            {
                nome = "Cliente Fluxo",
                telefone = "11988887777",
                rua = "Rua Teste",
                numero = "100",
                bairro = "Centro",
                cpf = "52998224725"
            })
        };
        cadastro.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var cadastroResponse = await _client.SendAsync(cadastro);
        Assert.Equal(HttpStatusCode.Created, cadastroResponse.StatusCode);

        var cliente = await cadastroResponse.Content.ReadFromJsonAsync<ClienteDto>();
        Assert.NotNull(cliente);

        var listagem = new HttpRequestMessage(HttpMethod.Get, "/api/clientes?pagina=1&tamanhoPagina=20");
        listagem.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var listagemResponse = await _client.SendAsync(listagem);

        Assert.Equal(HttpStatusCode.OK, listagemResponse.StatusCode);

        var pagina = await listagemResponse.Content.ReadFromJsonAsync<ResultadoPaginadoDto<ClienteDto>>();
        Assert.NotNull(pagina);
        Assert.True(pagina.TotalItens >= 1);
        Assert.Contains(pagina.Itens, c => c.Id == cliente!.Id);
    }

    [Fact]
    public async Task CadastrarCliente_SemNome_DeveRetornar400()
    {
        var token = await ObterTokenGerenteAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/clientes")
        {
            Content = JsonContent.Create(new
            {
                nome = "",
                telefone = "11988887777",
                rua = "Rua Teste",
                numero = "100",
                bairro = "Centro"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ListarClientes_TamanhoPaginaInvalido_DeveRetornar400()
    {
        var token = await ObterTokenGerenteAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/clientes?tamanhoPagina=500");
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
