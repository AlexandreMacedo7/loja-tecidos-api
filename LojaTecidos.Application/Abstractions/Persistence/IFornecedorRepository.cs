using LojaTecidos.Domain.Entities;

namespace LojaTecidos.Application.Abstractions.Persistence;

public interface IFornecedorRepository
{
    Task<Fornecedor?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Fornecedor>> ListarAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(Fornecedor fornecedor, CancellationToken cancellationToken = default);
}
