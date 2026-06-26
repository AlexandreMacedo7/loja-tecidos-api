using LojaTecidos.Application.Abstractions;
using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Application.Autenticacao;
using LojaTecidos.Application.Clientes;
using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Application.Common.Paginacao;
using LojaTecidos.Application.Produtos;
using LojaTecidos.Application.Services;
using LojaTecidos.Application.Vendas;
using LojaTecidos.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LojaTecidos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ServicoVenda>();
        services.AddSingleton<IGeradorCodigoVenda, GeradorCodigoVenda>();

        RegisterUseCase<CadastrarClienteRequest, ClienteDto, CadastrarClienteUseCase>(services);
        RegisterUseCase<ObterClienteRequest, ClienteDto?, ObterClienteUseCase>(services);
        RegisterUseCase<ListarClientesRequest, ResultadoPaginadoDto<ClienteDto>, ListarClientesUseCase>(services);
        RegisterUseCase<AlterarPerfilClienteRequest, ClienteDto, AlterarPerfilClienteUseCase>(services);
        RegisterUseCase<AlterarBloqueioClienteRequest, ClienteDto, AlterarBloqueioClienteUseCase>(services);
        RegisterUseCase<RegistrarPagamentoFiadoRequest, ClienteDto, RegistrarPagamentoFiadoUseCase>(services);

        RegisterUseCase<CadastrarProdutoRequest, ProdutoDto, CadastrarProdutoUseCase>(services);
        RegisterUseCase<ObterProdutoRequest, ProdutoDto?, ObterProdutoUseCase>(services);
        RegisterUseCase<ListarProdutosRequest, ResultadoPaginadoDto<ProdutoDto>, ListarProdutosUseCase>(services);
        RegisterUseCase<RegistrarEntradaEstoqueRequest, ProdutoDto, RegistrarEntradaEstoqueUseCase>(services);
        RegisterUseCase<AtualizarEstoqueRequest, ProdutoDto, AtualizarEstoqueUseCase>(services);
        RegisterUseCase<ListarAlertasEstoqueRequest, IReadOnlyList<AlertaEstoqueDto>, ListarAlertasEstoqueUseCase>(services);

        RegisterUseCase<RegistrarVendaAvistaRequest, VendaDto, RegistrarVendaAvistaUseCase>(services);
        RegisterUseCase<RegistrarVendaFiadoRequest, VendaDto, RegistrarVendaFiadoUseCase>(services);
        RegisterUseCase<RegistrarDevolucaoRequest, VendaDto, RegistrarDevolucaoUseCase>(services);
        RegisterUseCase<ObterVendaRequest, VendaDto?, ObterVendaUseCase>(services);

        RegisterUseCase<LoginUsuarioRequest, TokenAutenticacaoDto?, LoginUsuarioUseCase>(services);

        return services;
    }

    private static void RegisterUseCase<TRequest, TResponse, TImplementation>(IServiceCollection services)
        where TImplementation : class, IUseCase<TRequest, TResponse>
    {
        services.AddScoped<IUseCase<TRequest, TResponse>, TImplementation>();
        services.AddScoped<TImplementation>();
    }
}
