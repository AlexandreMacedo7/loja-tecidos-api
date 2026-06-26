using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Entities;

public static class RegraUnidadeMedidaProduto
{
    public static UnidadeMedida ObterUnidadeEsperada(CategoriaProduto categoria) =>
        categoria switch
        {
            CategoriaProduto.Tecido or CategoriaProduto.Cortinado => UnidadeMedida.Metro,
            CategoriaProduto.Travesseiro => UnidadeMedida.Par,
            _ => UnidadeMedida.Unidade
        };

    public static void Validar(CategoriaProduto categoria, UnidadeMedida unidadeMedida)
    {
        var unidadeEsperada = ObterUnidadeEsperada(categoria);

        if (unidadeMedida != unidadeEsperada)
            throw new InvalidOperationException(
                $"A categoria {categoria} exige unidade de medida {unidadeEsperada}.");
    }
}
