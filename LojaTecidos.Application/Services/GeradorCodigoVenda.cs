using LojaTecidos.Application.Abstractions;

namespace LojaTecidos.Application.Services;

public class GeradorCodigoVenda : IGeradorCodigoVenda
{
    public string Gerar(DateTime dataReferencia) =>
        $"VND-{dataReferencia:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
}
