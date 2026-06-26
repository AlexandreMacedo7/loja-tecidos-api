using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Entities;

public class PerfilCredito
{
    public CategoriaPerfil Categoria { get; }
    public decimal Limite { get; }

    public PerfilCredito(CategoriaPerfil categoria)
    {
        Categoria = categoria;
        Limite = categoria switch
        {
            CategoriaPerfil.OURO => 500.00m,
            CategoriaPerfil.PRATA => 300.00m,
            CategoriaPerfil.BRONZE => 150.00m,
            _ => 0
        };
    }
}
