using System;
using System.Collections.Generic;
using System.Text;

namespace LojaTecidos.Domain.Entities
{
    public class ContaFiado
    {
        public DateTime DataVencimento { get; private set; }
        public decimal SaldoDevedor { get; private set; }

        public ContaFiado(DateTime dateTime, decimal saldoDevedor) { 
            DataVencimento = dateTime;
            SaldoDevedor = saldoDevedor;
        
        }

        public void RegistrarPagamento(decimal valorRecebido)
        {
            if(valorRecebido <= 0) throw new ArgumentException("Pagamento não pode ser menor ou igual a R$ 0.00");

            if (SaldoDevedor < valorRecebido) throw new InvalidOperationException("O valor do pagamento não pode ser maior que o saldo devedor.");
            
            SaldoDevedor -= valorRecebido;
        }
    }
}
