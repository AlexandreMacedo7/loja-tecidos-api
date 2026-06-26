using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Entities;

public class ContaFiado
{
    public Guid Id { get; }
    public DateTime DataEmissao { get; private set; }
    public DateTime DataVencimento { get; private set; }
    public decimal SaldoDevedor { get; private set; }
    public StatusContaFiado Status { get; private set; }

    public bool EstaAtiva => Status == StatusContaFiado.Ativa;

    public ContaFiado(DateTime dataEmissao, DateTime dataVencimento, decimal saldoDevedor)
    {
        if (saldoDevedor <= 0)
            throw new ArgumentException("Saldo devedor deve ser maior que zero.", nameof(saldoDevedor));

        ValidarVencimentoEmRelacaoAEmissao(dataEmissao, dataVencimento);

        Id = Guid.NewGuid();
        DataEmissao = dataEmissao;
        DataVencimento = dataVencimento;
        SaldoDevedor = saldoDevedor;
        Status = StatusContaFiado.Ativa;
    }

    /// <summary>
    /// Reidrata a conta a partir da persistência. Uso exclusivo da camada Infrastructure.
    /// </summary>
    internal static ContaFiado Reconstituir(
        Guid id,
        DateTime dataEmissao,
        DateTime dataVencimento,
        decimal saldoDevedor,
        StatusContaFiado status) =>
        new(id, dataEmissao, dataVencimento, saldoDevedor, status);

    private ContaFiado(
        Guid id,
        DateTime dataEmissao,
        DateTime dataVencimento,
        decimal saldoDevedor,
        StatusContaFiado status)
    {
        ValidarEstadoPersistido(saldoDevedor, status);

        Id = id;
        DataEmissao = dataEmissao;
        DataVencimento = dataVencimento;
        SaldoDevedor = saldoDevedor;
        Status = status;
    }

    public void AdicionarCompra(decimal valor)
    {
        if (!EstaAtiva)
            throw new InvalidOperationException("Não é possível adicionar compra em conta encerrada.");

        if (valor <= 0)
            throw new ArgumentException("Valor da compra deve ser maior que zero.", nameof(valor));

        SaldoDevedor += valor;
    }

    public void RegistrarPagamento(decimal valorRecebido, DateTime dataPagamento, DateTime? novaDataVencimento = null)
    {
        if (!EstaAtiva)
            throw new InvalidOperationException("Não é possível registrar pagamento em conta encerrada.");

        if (valorRecebido <= 0)
            throw new ArgumentException("Pagamento deve ser maior que zero.");

        if (valorRecebido > SaldoDevedor)
            throw new InvalidOperationException("O valor do pagamento não pode ser maior que o saldo devedor.");

        SaldoDevedor -= valorRecebido;

        if (SaldoDevedor == 0)
        {
            Status = StatusContaFiado.Quitada;
            return;
        }

        if (novaDataVencimento is null)
            throw new InvalidOperationException("Pagamento parcial exige nova data de vencimento.");

        ValidarVencimentoEmRelacaoAPagamento(dataPagamento, novaDataVencimento.Value);
        DataVencimento = novaDataVencimento.Value;
    }

    private static void ValidarEstadoPersistido(decimal saldoDevedor, StatusContaFiado status)
    {
        if (status == StatusContaFiado.Ativa && saldoDevedor <= 0)
            throw new ArgumentException("Conta ativa persistida deve possuir saldo devedor maior que zero.");

        if (status == StatusContaFiado.Quitada && saldoDevedor != 0)
            throw new ArgumentException("Conta quitada persistida deve possuir saldo devedor zerado.");
    }

    private static void ValidarVencimentoEmRelacaoAEmissao(DateTime dataEmissao, DateTime dataVencimento)
    {
        var prazoMaximo = dataEmissao.AddDays(30);

        if (dataVencimento > prazoMaximo)
            throw new InvalidOperationException("A data de vencimento não pode ser superior a 30 dias da emissão.");
    }

    private static void ValidarVencimentoEmRelacaoAPagamento(DateTime dataPagamento, DateTime novaDataVencimento)
    {
        var prazoMaximo = dataPagamento.AddDays(30);

        if (novaDataVencimento > prazoMaximo)
            throw new InvalidOperationException("A nova data de vencimento não pode ser superior a 30 dias da data do pagamento.");
    }
}
