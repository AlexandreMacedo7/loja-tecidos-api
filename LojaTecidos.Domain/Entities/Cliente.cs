using System;
using System.Collections.Generic;
using System.Text;

namespace LojaTecidos.Domain.Entities
{
    public class Cliente
    {
        
        private List<ContaFiado> _contaFiados = new List<ContaFiado>();
        public IReadOnlyCollection<ContaFiado> Contas => _contaFiados.AsReadOnly();
        public PerfilCredito PerfilCredito { get; private set; }

        public Cliente(PerfilCredito perfil)
        {
            PerfilCredito = perfil;
        }

        public void AdicionarConta(ContaFiado conta)
        {
            if (_contaFiados.Count > 0)
            {
                throw new InvalidOperationException("Cliente já possuí dívida ativa.");
            }
            else if (conta.SaldoDevedor > PerfilCredito.RetornoSaldoPerfil())
            {
                throw new InvalidOperationException("Crédito insuficiente para cliente Bronze");
            }
            else
            {
                _contaFiados.Add(conta);
            }
            
        }
    }
}
