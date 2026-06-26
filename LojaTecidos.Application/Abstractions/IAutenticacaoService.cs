namespace LojaTecidos.Application.Abstractions;

public interface IAutenticacaoService
{
    Task<TokenAutenticacaoResultado?> AutenticarAsync(
        string email,
        string senha,
        CancellationToken cancellationToken = default);
}

public sealed record TokenAutenticacaoResultado(
    string Token,
    string Email,
    string Nome,
    IReadOnlyList<string> Papeis,
    DateTime ExpiraEm);
