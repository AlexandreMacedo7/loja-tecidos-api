namespace LojaTecidos.Application.Abstractions;

public interface IGeradorCodigoVenda
{
    string Gerar(DateTime dataReferencia);
}
