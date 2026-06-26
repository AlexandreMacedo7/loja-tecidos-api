using LojaTecidos.Application.Common.Dtos;
using LojaTecidos.Domain.Entities;

namespace LojaTecidos.Application.Common.Mappings;

internal static class DomainMapper
{
    public static ClienteDto ToDto(Cliente cliente) =>
        new(
            cliente.Id,
            cliente.Nome,
            cliente.Telefone,
            new EnderecoDto(
                cliente.Endereco.Rua,
                cliente.Endereco.Numero,
                cliente.Endereco.Bairro,
                cliente.Endereco.Cep),
            cliente.Cpf,
            cliente.Cnpj,
            cliente.PerfilCredito.Categoria,
            cliente.PerfilCredito.Limite,
            cliente.Bloqueado,
            cliente.ContaFiadoAtiva is null
                ? null
                : new ContaFiadoDto(
                    cliente.ContaFiadoAtiva.Id,
                    cliente.ContaFiadoAtiva.DataEmissao,
                    cliente.ContaFiadoAtiva.DataVencimento,
                    cliente.ContaFiadoAtiva.SaldoDevedor,
                    cliente.ContaFiadoAtiva.Status));

    public static ProdutoDto ToDto(Produto produto, DateTime dataReferenciaAlertas) =>
        new(
            produto.Id,
            produto.CodigoInterno,
            produto.CodigoFornecedor,
            produto.Nome,
            produto.Fornecedor.Nome,
            produto.Categoria,
            produto.UnidadeMedida,
            produto.PrecoUnitario,
            produto.Descontinuado,
            produto.QuantidadeAtual,
            produto.QuantidadeReferenciaAlerta,
            produto.EstaProximoDoFim(),
            produto.EstaParado(dataReferenciaAlertas));

    public static VendaDto ToDto(Venda venda) =>
        new(
            venda.Id,
            venda.NumeroSequencial,
            venda.CodigoVenda,
            venda.DataVenda,
            venda.Tipo,
            venda.Status,
            venda.DescontoPercentual,
            venda.TotalBruto,
            venda.ValorDesconto,
            venda.TotalLiquido,
            venda.UsuarioId,
            venda.Itens
                .Select(i => new ItemVendaDto(
                    i.CodigoInternoProduto,
                    i.Quantidade,
                    i.PrecoUnitario,
                    i.Subtotal))
                .ToList());
}
