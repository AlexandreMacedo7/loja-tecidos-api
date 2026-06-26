using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Entities;

public class Venda
{
    public const decimal DescontoMaximoPercentual = 20m;
    public const int PrazoDevolucaoDias = 7;

    private readonly List<ItemVenda> _itens;

    public int NumeroSequencial { get; }
    public string CodigoVenda { get; }
    public DateTime DataVenda { get; }
    public TipoVenda Tipo { get; }
    public StatusVenda Status { get; private set; }
    public decimal DescontoPercentual { get; }
    public string? UsuarioId { get; }
    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

    public decimal TotalBruto => decimal.Round(_itens.Sum(i => i.Subtotal), 2);

    public decimal ValorDesconto =>
        decimal.Round(TotalBruto * DescontoPercentual / 100m, 2);

    public decimal TotalLiquido => decimal.Round(TotalBruto - ValorDesconto, 2);

    private Venda(
        int numeroSequencial,
        string codigoVenda,
        DateTime dataVenda,
        TipoVenda tipo,
        decimal descontoPercentual,
        IEnumerable<ItemVenda> itens,
        string? usuarioId)
    {
        if (numeroSequencial <= 0)
            throw new ArgumentException("Número sequencial deve ser maior que zero.", nameof(numeroSequencial));

        if (string.IsNullOrWhiteSpace(codigoVenda))
            throw new ArgumentException("Código da venda é obrigatório.", nameof(codigoVenda));

        if (descontoPercentual < 0 || descontoPercentual > DescontoMaximoPercentual)
            throw new ArgumentException($"Desconto máximo permitido é de {DescontoMaximoPercentual}%.");

        _itens = itens.ToList();

        if (_itens.Count == 0)
            throw new ArgumentException("A venda deve conter ao menos um item.");

        NumeroSequencial = numeroSequencial;
        CodigoVenda = codigoVenda.Trim();
        DataVenda = dataVenda;
        Tipo = tipo;
        DescontoPercentual = decimal.Round(descontoPercentual, 2);
        UsuarioId = string.IsNullOrWhiteSpace(usuarioId) ? null : usuarioId.Trim();
        Status = StatusVenda.Confirmada;
    }

    public static Venda CriarAvista(
        int numeroSequencial,
        string codigoVenda,
        DateTime dataVenda,
        IEnumerable<ItemVenda> itens,
        decimal descontoPercentual = 0,
        string? usuarioId = null) =>
        new(numeroSequencial, codigoVenda, dataVenda, TipoVenda.AVista, descontoPercentual, itens, usuarioId);

    public static Venda CriarFiado(
        int numeroSequencial,
        string codigoVenda,
        DateTime dataVenda,
        IEnumerable<ItemVenda> itens,
        decimal descontoPercentual = 0,
        string? usuarioId = null) =>
        new(numeroSequencial, codigoVenda, dataVenda, TipoVenda.Fiado, descontoPercentual, itens, usuarioId);

    public void ValidarPrazoDevolucao(DateTime dataDevolucao)
    {
        if (Status == StatusVenda.Devolvida)
            throw new InvalidOperationException("Venda já foi devolvida.");

        if ((dataDevolucao.Date - DataVenda.Date).Days > PrazoDevolucaoDias)
            throw new InvalidOperationException($"Devolução permitida em até {PrazoDevolucaoDias} dias.");
    }

    internal void MarcarComoDevolvida() => Status = StatusVenda.Devolvida;
}
