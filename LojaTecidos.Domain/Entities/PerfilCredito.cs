using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Domain.Entities
{
    public class PerfilCredito
    {
        public CategoriaPerfil Categoria { get; set; }
        private decimal limiteBronze = 150.00m;

        public PerfilCredito(CategoriaPerfil categoria) {
        
            Categoria = categoria;
        }

        public decimal RetornoSaldoPerfil()
        {
            return limiteBronze;

        }
    }
}