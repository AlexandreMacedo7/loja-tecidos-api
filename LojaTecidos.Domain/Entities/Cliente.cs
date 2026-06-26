using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.ValueObjects;

namespace LojaTecidos.Domain.Entities;

public class Cliente
{
    private readonly List<ContaFiado> _contasQuitadas = [];

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
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone é obrigatório.", nameof(telefone));

        if (!string.IsNullOrWhiteSpace(cpf) && !string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("Informe apenas CPF ou CNPJ.");

        Nome = nome.Trim();
        Telefone = telefone.Trim();
        Endereco = endereco;
        Cpf = string.IsNullOrWhiteSpace(cpf) ? null : cpf.Trim();
        Cnpj = string.IsNullOrWhiteSpace(cnpj) ? null : cnpj.Trim();
        PerfilCredito = perfilCredito ?? new PerfilCredito(CategoriaPerfil.BRONZE);
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
}
