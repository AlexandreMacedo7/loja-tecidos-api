using LojaTecidos.Domain.Entities;
using LojaTecidos.Infrastructure.Persistence.Entities;

namespace LojaTecidos.Infrastructure.Persistence.Mappers;

internal static class ProdutoMapper
{
    public static Produto ToDomain(ProdutoEntity entity)
    {
        var movimentacoes = entity.Movimentacoes
            .OrderBy(m => m.Data)
            .Select(m => new MovimentacaoEstoque(m.Tipo, m.Quantidade, m.SaldoAposMovimentacao, m.Data))
            .ToList();

        return Produto.Reconstituir(
            entity.Id,
            entity.CodigoInterno,
            entity.Nome,
            FornecedorMapper.ToDomain(entity.Fornecedor),
            entity.Categoria,
            entity.UnidadeMedida,
            entity.PrecoUnitario,
            entity.QuantidadeAtual,
            entity.QuantidadeReferenciaAlerta,
            entity.DataCadastro,
            entity.DataUltimaVenda,
            entity.DataUltimaEntrada,
            entity.Descontinuado,
            entity.CodigoFornecedor,
            movimentacoes);
    }

    public static ProdutoEntity ToEntity(Produto produto, Guid fornecedorId)
    {
        var entity = new ProdutoEntity
        {
            Id = produto.Id,
            CodigoInterno = produto.CodigoInterno,
            CodigoFornecedor = produto.CodigoFornecedor,
            Nome = produto.Nome,
            FornecedorId = fornecedorId,
            Categoria = produto.Categoria,
            UnidadeMedida = produto.UnidadeMedida,
            PrecoUnitario = produto.PrecoUnitario,
            Descontinuado = produto.Descontinuado,
            QuantidadeAtual = produto.QuantidadeAtual,
            QuantidadeReferenciaAlerta = produto.QuantidadeReferenciaAlerta,
            DataCadastro = produto.DataCadastro,
            DataUltimaVenda = produto.DataUltimaVenda,
            DataUltimaEntrada = produto.DataUltimaEntrada
        };

        SincronizarMovimentacoes(produto, entity);
        return entity;
    }

    public static void UpdateEntity(Produto produto, ProdutoEntity entity, Guid fornecedorId)
    {
        entity.CodigoInterno = produto.CodigoInterno;
        entity.CodigoFornecedor = produto.CodigoFornecedor;
        entity.Nome = produto.Nome;
        entity.FornecedorId = fornecedorId;
        entity.Categoria = produto.Categoria;
        entity.UnidadeMedida = produto.UnidadeMedida;
        entity.PrecoUnitario = produto.PrecoUnitario;
        entity.Descontinuado = produto.Descontinuado;
        entity.QuantidadeAtual = produto.QuantidadeAtual;
        entity.QuantidadeReferenciaAlerta = produto.QuantidadeReferenciaAlerta;
        entity.DataCadastro = produto.DataCadastro;
        entity.DataUltimaVenda = produto.DataUltimaVenda;
        entity.DataUltimaEntrada = produto.DataUltimaEntrada;

        SincronizarMovimentacoes(produto, entity);
    }

    private static void SincronizarMovimentacoes(Produto produto, ProdutoEntity entity)
    {
        if (produto.Movimentacoes.Count <= entity.Movimentacoes.Count)
            return;

        var novasMovimentacoes = produto.Movimentacoes.Skip(entity.Movimentacoes.Count);

        foreach (var movimentacao in novasMovimentacoes)
        {
            entity.Movimentacoes.Add(new MovimentacaoEstoqueEntity
            {
                Id = Guid.NewGuid(),
                ProdutoId = entity.Id,
                Tipo = movimentacao.Tipo,
                Quantidade = movimentacao.Quantidade,
                SaldoAposMovimentacao = movimentacao.SaldoAposMovimentacao,
                Data = movimentacao.Data
            });
        }
    }
}
