using System;
using System.Collections.Generic;
using System.Text;

namespace LojaTecidos.Domain.Entities
{
    public class ContaFiado
    {
        public DateTime DataEmissao { get; private set; }
        public DateTime DataVencimento { get; private set; }
        public decimal SaldoDevedor { get; private set; }

        public ContaFiado(DateTime dataEmissao, DateTime dataVencimento, decimal saldoDevedor) {

            var prazoMaximo = dataEmissao.AddDays(30);

            if (dataVencimento > prazoMaximo) {
                throw new InvalidOperationException("A Data de vencimento não pode ser superior a 30 dias");
            }

            DataEmissao = dataEmissao;
            DataVencimento = dataVencimento;
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
