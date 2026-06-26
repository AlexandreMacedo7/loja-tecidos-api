namespace LojaTecidos.Application.Abstractions.Persistence;

public sealed record ResultadoConsultaPaginada<T>(IReadOnlyList<T> Itens, int TotalItens);
