namespace LojaTecidos.Domain.ValueObjects;

public static class DocumentoIdentificacao
{
    public const int TamanhoCpf = 11;
    public const int TamanhoCnpj = 14;

    public static string? NormalizarCpf(string? cpf) =>
        Normalizar(cpf, TamanhoCpf, "CPF");

    public static string? NormalizarCnpj(string? cnpj) =>
        Normalizar(cnpj, TamanhoCnpj, "CNPJ");

    private static string? Normalizar(string? documento, int tamanhoEsperado, string nome)
    {
        if (string.IsNullOrWhiteSpace(documento))
            return null;

        var digitos = ExtrairDigitos(documento);
        if (digitos is null)
            return null;

        if (digitos.Length != tamanhoEsperado)
            throw new ArgumentException($"{nome} deve conter {tamanhoEsperado} dígitos.");

        return digitos;
    }

    private static string? ExtrairDigitos(string documento)
    {
        var digitos = new string(documento.Where(char.IsDigit).ToArray());
        return digitos.Length == 0 ? null : digitos;
    }
}
