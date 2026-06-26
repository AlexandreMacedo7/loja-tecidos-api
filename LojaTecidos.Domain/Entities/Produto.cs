using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Entities;

public class Produto
{
    private const decimal PercentualAlertaEstoqueBaixo = 0.20m;
    private const int DiasPadraoEstoqueParado = 45;

    private readonly List<MovimentacaoEstoque> _movimentacoes = [];

    public string CodigoInterno { get; }
    public string? CodigoFornecedor { get; }
    public string Nome { get; }
    public Fornecedor Fornecedor { get; }
    public CategoriaProduto Categoria { get; }
    public UnidadeMedida UnidadeMedida { get; }
    public decimal PrecoUnitario { get; private set; }
    public bool Descontinuado { get; private set; }
    public decimal QuantidadeAtual { get; private set; }
    public decimal QuantidadeReferenciaAlerta { get; private set; }
    public DateTime DataCadastro { get; }
    public DateTime? DataUltimaVenda { get; private set; }
    public DateTime? DataUltimaEntrada { get; private set; }
    public IReadOnlyCollection<MovimentacaoEstoque> Movimentacoes => _movimentacoes.AsReadOnly();

    public Produto(
        string codigoInterno,
        string nome,
        Fornecedor fornecedor,
        CategoriaProduto categoria,
        UnidadeMedida unidadeMedida,
        decimal precoUnitario,
        decimal estoqueInicial,
        DateTime dataCadastro,
        string? codigoFornecedor = null)
    {
        if (string.IsNullOrWhiteSpace(codigoInterno))
            throw new ArgumentException("Código interno é obrigatório.", nameof(codigoInterno));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório.", nameof(nome));

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero.", nameof(precoUnitario));

        if (estoqueInicial < 0)
            throw new ArgumentException("Estoque inicial não pode ser negativo.", nameof(estoqueInicial));

        RegraUnidadeMedidaProduto.Validar(categoria, unidadeMedida);

        CodigoInterno = codigoInterno.Trim();
        CodigoFornecedor = string.IsNullOrWhiteSpace(codigoFornecedor) ? null : codigoFornecedor.Trim();
        Nome = nome.Trim();
        Fornecedor = fornecedor;
        Categoria = categoria;
        UnidadeMedida = unidadeMedida;
        PrecoUnitario = decimal.Round(precoUnitario, 2);
        DataCadastro = dataCadastro;
        QuantidadeAtual = decimal.Round(estoqueInicial, 2);
        QuantidadeReferenciaAlerta = QuantidadeAtual;

        if (estoqueInicial > 0)
            DataUltimaEntrada = dataCadastro;

        if (estoqueInicial > 0)
            RegistrarMovimentacao(TipoMovimentacaoEstoque.Entrada, estoqueInicial, dataCadastro);
    }

    public void AlterarPreco(decimal novoPreco)
    {
        if (novoPreco <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero.", nameof(novoPreco));

        PrecoUnitario = decimal.Round(novoPreco, 2);
    }

    public void Descontinuar() => Descontinuado = true;

    public void RegistrarEntrada(decimal quantidade, DateTime data)
    {
        ValidarQuantidade(quantidade);

        QuantidadeAtual = decimal.Round(QuantidadeAtual + quantidade, 2);
        DataUltimaEntrada = data;
        RegistrarMovimentacao(TipoMovimentacaoEstoque.Entrada, quantidade, data);
    }

    public void RegistrarSaida(decimal quantidade, DateTime data)
    {
        ValidarQuantidade(quantidade);

        if (quantidade > QuantidadeAtual)
            throw new InvalidOperationException("Quantidade insuficiente em estoque.");

        QuantidadeAtual = decimal.Round(QuantidadeAtual - quantidade, 2);
        DataUltimaVenda = data;
        RegistrarMovimentacao(TipoMovimentacaoEstoque.Saida, quantidade, data);
    }

    public void AtualizarEstoque(decimal novaQuantidade, DateTime data)
    {
        if (novaQuantidade < 0)
            throw new ArgumentException("Estoque não pode ser negativo.", nameof(novaQuantidade));

        var quantidadeAnterior = QuantidadeAtual;

        if (novaQuantidade == quantidadeAnterior)
            return;

        QuantidadeAtual = decimal.Round(novaQuantidade, 2);

        if (novaQuantidade > quantidadeAnterior)
        {
            QuantidadeReferenciaAlerta = QuantidadeAtual;
            DataUltimaEntrada = data;
        }

        var diferenca = decimal.Round(Math.Abs(novaQuantidade - quantidadeAnterior), 2);
        RegistrarMovimentacao(TipoMovimentacaoEstoque.Ajuste, diferenca, data);
    }

    public bool EstaProximoDoFim()
    {
        if (QuantidadeReferenciaAlerta <= 0)
            return false;

        var limiteAlerta = decimal.Round(QuantidadeReferenciaAlerta * PercentualAlertaEstoqueBaixo, 2);
        return QuantidadeAtual <= limiteAlerta;
    }

    public bool EstaParado(DateTime dataReferencia, int diasParado = DiasPadraoEstoqueParado)
    {
        var dataUltimaMovimentacaoRelevante = ObterDataReferenciaParado();
        return (dataReferencia - dataUltimaMovimentacaoRelevante).TotalDays >= diasParado;
    }

    private DateTime ObterDataReferenciaParado()
    {
        if (DataUltimaVenda.HasValue)
            return DataUltimaVenda.Value;

        if (DataUltimaEntrada.HasValue)
            return DataUltimaEntrada.Value;

        return DataCadastro;
    }

    private void ValidarQuantidade(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
    }

    private void RegistrarMovimentacao(TipoMovimentacaoEstoque tipo, decimal quantidade, DateTime data)
    {
        _movimentacoes.Add(new MovimentacaoEstoque(tipo, decimal.Round(quantidade, 2), QuantidadeAtual, data));
    }
}
