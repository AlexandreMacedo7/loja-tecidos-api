using LojaTecidos.Domain.Entities;

namespace LojaTecidos.Application.Abstractions.Persistence;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorCodigoInternoAsync(string codigoInterno, CancellationToken cancellationToken = default);
    Task<ResultadoConsultaPaginada<Produto>> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Produto>> ObterPorCodigosInternosAsync(
        IEnumerable<string> codigosInternos,
        CancellationToken cancellationToken = default);
    Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Produto produto, CancellationToken cancellationToken = default);
}
