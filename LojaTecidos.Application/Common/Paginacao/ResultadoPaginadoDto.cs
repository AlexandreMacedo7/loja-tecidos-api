namespace LojaTecidos.Application.Common.Paginacao;

public sealed record ResultadoPaginadoDto<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens,
    int TotalPaginas);

public static class PaginacaoParametros
{
    public const int TamanhoPaginaPadrao = 20;
    public const int TamanhoPaginaMaximo = 100;

    public static (int Pagina, int TamanhoPagina) Normalizar(int pagina, int tamanhoPagina)
    {
        if (pagina < 1)
            throw new ArgumentException("O parâmetro 'pagina' deve ser maior ou igual a 1.");

        if (tamanhoPagina < 1 || tamanhoPagina > TamanhoPaginaMaximo)
            throw new ArgumentException(
                $"O parâmetro 'tamanhoPagina' deve estar entre 1 e {TamanhoPaginaMaximo}.");

        return (pagina, tamanhoPagina);
    }

    public static ResultadoPaginadoDto<T> Criar<T>(
        IReadOnlyList<T> itens,
        int pagina,
        int tamanhoPagina,
        int totalItens) =>
        new(
            itens,
            pagina,
            tamanhoPagina,
            totalItens,
            totalItens == 0 ? 0 : (int)Math.Ceiling(totalItens / (double)tamanhoPagina));
}
