namespace LojaTecidos.Domain.ValueObjects;

public class Endereco
{
    public const string CepPadrao = "69260000";

    public string Rua { get; }
    public string Numero { get; }
    public string Bairro { get; }
    public string Cep { get; }

    public Endereco(string rua, string numero, string bairro, string cep = CepPadrao)
    {
        if (string.IsNullOrWhiteSpace(rua))
            throw new ArgumentException("Rua é obrigatória.", nameof(rua));

        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número é obrigatório.", nameof(numero));

        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("Bairro é obrigatório.", nameof(bairro));

        if (string.IsNullOrWhiteSpace(cep))
            throw new ArgumentException("CEP é obrigatório.", nameof(cep));

        Rua = rua.Trim();
        Numero = numero.Trim();
        Bairro = bairro.Trim();
        Cep = cep.Trim();
    }
}
