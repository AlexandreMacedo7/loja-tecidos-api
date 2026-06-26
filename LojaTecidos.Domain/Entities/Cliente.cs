using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Domain.Entities;

public class Cliente
{
    private readonly List<ContaFiado> _contasQuitadas = [];

    public Guid Id { get; }
    public string Nome { get; private set; }
    public string Telefone { get; private set; }
    public Endereco Endereco { get; private set; }
    public string? Cpf { get; private set; }
    public string? Cnpj { get; private set; }
    public PerfilCredito PerfilCredito { get; private set; }
    public bool Bloqueado { get; private set; }
    public ContaFiado? ContaFiadoAtiva { get; private set; }
    public IReadOnlyCollection<ContaFiado> ContasQuitadas => _contasQuitadas.AsReadOnly();

    public Cliente(
        string nome,
        string telefone,
        Endereco endereco,
        string? cpf = null,
        string? cnpj = null,
        PerfilCredito? perfilCredito = null)
    {
        ValidarDadosCadastro(nome, telefone, cpf, cnpj);

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        Telefone = telefone.Trim();
        Endereco = endereco;
        Cpf = DocumentoIdentificacao.NormalizarCpf(cpf);
        Cnpj = DocumentoIdentificacao.NormalizarCnpj(cnpj);
        PerfilCredito = perfilCredito ?? new PerfilCredito(CategoriaPerfil.BRONZE);
    }

    /// <summary>
    /// Reidrata o cliente a partir da persistência. Uso exclusivo da camada Infrastructure.
    /// </summary>
    internal static Cliente Reconstituir(
        Guid id,
        string nome,
        string telefone,
        Endereco endereco,
        string? cpf,
        string? cnpj,
        CategoriaPerfil categoriaPerfil,
        bool bloqueado,
        ContaFiado? contaFiadoAtiva,
        IEnumerable<ContaFiado> contasQuitadas) =>
        new(
            id,
            nome,
            telefone,
            endereco,
            cpf,
            cnpj,
            categoriaPerfil,
            bloqueado,
            contaFiadoAtiva,
            contasQuitadas);

    private Cliente(
        Guid id,
        string nome,
        string telefone,
        Endereco endereco,
        string? cpf,
        string? cnpj,
        CategoriaPerfil categoriaPerfil,
        bool bloqueado,
        ContaFiado? contaFiadoAtiva,
        IEnumerable<ContaFiado> contasQuitadas)
    {
        ValidarDadosCadastro(nome, telefone, cpf, cnpj);

        Id = id;
        Nome = nome.Trim();
        Telefone = telefone.Trim();
        Endereco = endereco;
        Cpf = DocumentoIdentificacao.NormalizarCpf(cpf);
        Cnpj = DocumentoIdentificacao.NormalizarCnpj(cnpj);
        PerfilCredito = new PerfilCredito(categoriaPerfil);
        Bloqueado = bloqueado;
        ContaFiadoAtiva = contaFiadoAtiva;
        _contasQuitadas.AddRange(contasQuitadas);
    }

    public void AlterarPerfil(CategoriaPerfil categoria)
    {
        PerfilCredito = new PerfilCredito(categoria);
    }

    public void Bloquear() => Bloqueado = true;

    public void Desbloquear() => Bloqueado = false;

    public void RegistrarCompraFiado(decimal valor, DateTime dataEmissao, DateTime dataVencimento)
    {
        if (Bloqueado)
            throw new InvalidOperationException("Cliente bloqueado não pode comprar a fiado.");

        if (valor <= 0)
            throw new ArgumentException("Valor da compra deve ser maior que zero.", nameof(valor));

        if (ContaFiadoAtiva is null)
        {
            if (valor > PerfilCredito.Limite)
                throw new InvalidOperationException("Crédito insuficiente para o perfil do cliente.");

            ContaFiadoAtiva = new ContaFiado(dataEmissao, dataVencimento, valor);
            return;
        }

        if (ContaFiadoAtiva.SaldoDevedor + valor > PerfilCredito.Limite)
            throw new InvalidOperationException("Crédito insuficiente para o perfil do cliente.");

        ContaFiadoAtiva.AdicionarCompra(valor);
    }

    public void RegistrarPagamentoFiado(decimal valor, DateTime dataPagamento, DateTime? novaDataVencimento = null)
    {
        if (ContaFiadoAtiva is null)
            throw new InvalidOperationException("Cliente não possui conta fiado ativa.");

        ContaFiadoAtiva.RegistrarPagamento(valor, dataPagamento, novaDataVencimento);

        if (!ContaFiadoAtiva.EstaAtiva)
        {
            _contasQuitadas.Add(ContaFiadoAtiva);
            ContaFiadoAtiva = null;
        }
    }

    private static void ValidarDadosCadastro(string nome, string telefone, string? cpf, string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone é obrigatório.", nameof(telefone));

        if (!string.IsNullOrWhiteSpace(cpf) && !string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("Informe apenas CPF ou CNPJ.");
    }

}
