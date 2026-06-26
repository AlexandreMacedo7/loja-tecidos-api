using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Exceptions;
using LojaTecidos.Infrastructure.Persistence.Entities;
using LojaTecidos.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace LojaTecidos.Infrastructure.Persistence.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly LojaTecidosDbContext _context;

    public ProdutoRepository(LojaTecidosDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> ObterPorCodigoInternoAsync(
        string codigoInterno,
        CancellationToken cancellationToken = default)
    {
        var entity = await ObterQueryCompleta()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CodigoInterno == codigoInterno.Trim(), cancellationToken);

        return entity is null ? null : ProdutoMapper.ToDomain(entity);
    }

    public async Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var entities = await ObterQueryCompleta()
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);

        return entities.Select(ProdutoMapper.ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Produto>> ObterPorCodigosInternosAsync(
        IEnumerable<string> codigosInternos,
        CancellationToken cancellationToken = default)
    {
        var codigos = codigosInternos.Select(c => c.Trim()).Distinct().ToList();

        var entities = await ObterQueryCompleta()
            .AsNoTracking()
            .Where(p => codigos.Contains(p.CodigoInterno))
            .ToListAsync(cancellationToken);

        return entities.Select(ProdutoMapper.ToDomain).ToList();
    }

    public async Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        var fornecedorId = await ObterOuCriarFornecedorIdAsync(produto.Fornecedor, cancellationToken);
        var entity = ProdutoMapper.ToEntity(produto, fornecedorId);
        await _context.Produtos.AddAsync(entity, cancellationToken);
    }

    public async Task AtualizarAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == produto.Id, cancellationToken);

        if (entity is null)
            throw new EntidadeNaoEncontradaException($"Produto {produto.CodigoInterno} não encontrado.");

        var fornecedorId = await ObterOuCriarFornecedorIdAsync(produto.Fornecedor, cancellationToken);

        entity.PrecoUnitario = produto.PrecoUnitario;
        entity.Descontinuado = produto.Descontinuado;
        entity.QuantidadeAtual = produto.QuantidadeAtual;
        entity.QuantidadeReferenciaAlerta = produto.QuantidadeReferenciaAlerta;
        entity.DataUltimaVenda = produto.DataUltimaVenda;
        entity.DataUltimaEntrada = produto.DataUltimaEntrada;
        entity.FornecedorId = fornecedorId;

        var quantidadeMovimentacoes = await _context.MovimentacoesEstoque
            .CountAsync(m => m.ProdutoId == entity.Id, cancellationToken);

        if (produto.Movimentacoes.Count > quantidadeMovimentacoes)
        {
            foreach (var movimentacao in produto.Movimentacoes.Skip(quantidadeMovimentacoes))
            {
                await _context.MovimentacoesEstoque.AddAsync(new MovimentacaoEstoqueEntity
                {
                    Id = Guid.NewGuid(),
                    ProdutoId = entity.Id,
                    Tipo = movimentacao.Tipo,
                    Quantidade = movimentacao.Quantidade,
                    SaldoAposMovimentacao = movimentacao.SaldoAposMovimentacao,
                    Data = movimentacao.Data
                }, cancellationToken);
            }
        }
    }

    private IQueryable<ProdutoEntity> ObterQueryCompleta() =>
        _context.Produtos
            .Include(p => p.Fornecedor)
            .Include(p => p.Movimentacoes);

    private async Task<Guid> ObterOuCriarFornecedorIdAsync(
        Fornecedor fornecedor,
        CancellationToken cancellationToken)
    {
        var entity = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Id == fornecedor.Id || f.Nome == fornecedor.Nome, cancellationToken);

        if (entity is not null)
            return entity.Id;

        entity = FornecedorMapper.ToEntity(fornecedor);
        await _context.Fornecedores.AddAsync(entity, cancellationToken);
        return entity.Id;
    }
}
