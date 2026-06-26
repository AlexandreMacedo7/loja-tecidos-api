using LojaTecidos.Domain.Entities;

namespace LojaTecidos.Application.Abstractions.Persistence;

public interface IVendaRepository
{
    Task<Venda?> ObterPorCodigoAsync(string codigoVenda, CancellationToken cancellationToken = default);
    Task<int> ObterProximoNumeroSequencialAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(Venda venda, Guid? clienteId, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Venda venda, CancellationToken cancellationToken = default);
}
