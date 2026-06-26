using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Entities;

public class MovimentacaoEstoque
{
    public TipoMovimentacaoEstoque Tipo { get; }
    public decimal Quantidade { get; }
    public decimal SaldoAposMovimentacao { get; }
    public DateTime Data { get; }

    public MovimentacaoEstoque(
        TipoMovimentacaoEstoque tipo,
        decimal quantidade,
        decimal saldoAposMovimentacao,
        DateTime data)
    {
        Tipo = tipo;
        Quantidade = quantidade;
        SaldoAposMovimentacao = saldoAposMovimentacao;
        Data = data;
    }
}
