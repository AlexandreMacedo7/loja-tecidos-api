namespace LojaTecidos.Domain.Exceptions;

public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem)
        : base(mensagem)
    {
    }
}
