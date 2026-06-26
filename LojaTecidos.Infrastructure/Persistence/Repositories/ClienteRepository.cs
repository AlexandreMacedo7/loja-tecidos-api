using LojaTecidos.Application.Abstractions.Persistence;
using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Exceptions;
using LojaTecidos.Infrastructure.Persistence.Entities;
using LojaTecidos.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace LojaTecidos.Infrastructure.Persistence.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly LojaTecidosDbContext _context;

    public ClienteRepository(LojaTecidosDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Clientes
            .AsNoTracking()
            .Include(c => c.ContasFiado)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return entity is null ? null : ClienteMapper.ToDomain(entity);
    }

    public async Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Clientes
            .AsNoTracking()
            .Include(c => c.ContasFiado)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);

        return entities.Select(ClienteMapper.ToDomain).ToList();
    }

    public async Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        var entity = ClienteMapper.ToEntity(cliente);
        await _context.Clientes.AddAsync(entity, cancellationToken);
    }

    public async Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Clientes
            .Include(c => c.ContasFiado)
            .FirstOrDefaultAsync(c => c.Id == cliente.Id, cancellationToken);

        if (entity is null)
            throw new EntidadeNaoEncontradaException($"Cliente {cliente.Id} não encontrado.");

        entity.Nome = cliente.Nome;
        entity.Telefone = cliente.Telefone;
        entity.Rua = cliente.Endereco.Rua;
        entity.Numero = cliente.Endereco.Numero;
        entity.Bairro = cliente.Endereco.Bairro;
        entity.Cep = cliente.Endereco.Cep;
        entity.Cpf = cliente.Cpf;
        entity.Cnpj = cliente.Cnpj;
        entity.CategoriaPerfil = cliente.PerfilCredito.Categoria;
        entity.Bloqueado = cliente.Bloqueado;

        await SincronizarContasFiadoAsync(cliente, entity, cancellationToken);
    }

    private async Task SincronizarContasFiadoAsync(
        Cliente cliente,
        ClienteEntity entity,
        CancellationToken cancellationToken)
    {
        var contasDomain = new List<ContaFiado>();
        if (cliente.ContaFiadoAtiva is not null)
            contasDomain.Add(cliente.ContaFiadoAtiva);

        contasDomain.AddRange(cliente.ContasQuitadas);

        var idsDomain = contasDomain.Select(c => c.Id).ToHashSet();
        var contasRemover = entity.ContasFiado.Where(c => !idsDomain.Contains(c.Id)).ToList();

        foreach (var conta in contasRemover)
            _context.ContasFiado.Remove(conta);

        foreach (var conta in contasDomain)
        {
            var existente = await _context.ContasFiado
                .FirstOrDefaultAsync(c => c.Id == conta.Id, cancellationToken);

            if (existente is null)
            {
                await _context.ContasFiado.AddAsync(new ContaFiadoEntity
                {
                    Id = conta.Id,
                    ClienteId = entity.Id,
                    DataEmissao = conta.DataEmissao,
                    DataVencimento = conta.DataVencimento,
                    SaldoDevedor = conta.SaldoDevedor,
                    Status = conta.Status
                }, cancellationToken);

                continue;
            }

            existente.DataEmissao = conta.DataEmissao;
            existente.DataVencimento = conta.DataVencimento;
            existente.SaldoDevedor = conta.SaldoDevedor;
            existente.Status = conta.Status;
            existente.ClienteId = entity.Id;
        }
    }
}
