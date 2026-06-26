using LojaTecidos.Domain.Entities;
using LojaTecidos.Domain.Entities.Enum;
using LojaTecidos.Domain.ValueObjects;
using LojaTecidos.Infrastructure.Persistence.Entities;

namespace LojaTecidos.Infrastructure.Persistence.Mappers;

internal static class ClienteMapper
{
    public static Cliente ToDomain(ClienteEntity entity)
    {
        var contaAtiva = entity.ContasFiado
            .FirstOrDefault(c => c.Status == StatusContaFiado.Ativa);

        var contasQuitadas = entity.ContasFiado
            .Where(c => c.Status == StatusContaFiado.Quitada)
            .Select(ToContaDomain)
            .ToList();

        return Cliente.Reconstituir(
            entity.Id,
            entity.Nome,
            entity.Telefone,
            new Endereco(entity.Rua, entity.Numero, entity.Bairro, entity.Cep),
            entity.Cpf,
            entity.Cnpj,
            entity.CategoriaPerfil,
            entity.Bloqueado,
            contaAtiva is null ? null : ToContaDomain(contaAtiva),
            contasQuitadas);
    }

    public static ClienteEntity ToEntity(Cliente cliente)
    {
        var entity = new ClienteEntity
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Telefone = cliente.Telefone,
            Rua = cliente.Endereco.Rua,
            Numero = cliente.Endereco.Numero,
            Bairro = cliente.Endereco.Bairro,
            Cep = cliente.Endereco.Cep,
            Cpf = cliente.Cpf,
            Cnpj = cliente.Cnpj,
            CategoriaPerfil = cliente.PerfilCredito.Categoria,
            Bloqueado = cliente.Bloqueado
        };

        SincronizarContasFiado(cliente, entity);
        return entity;
    }

    public static void UpdateEntity(Cliente cliente, ClienteEntity entity)
    {
        entity.Nome = cliente.Nome;
        entity.Telefone = cliente.Telefone;
        entity.Rua = cliente.Endereco.Rua;
        entity.Numero = cliente.Endereco.Numero;
        entity.Bairro = cliente.Endereco.Bairro;
        entity.Cep = cliente.Endereco.Cep;
        entity.Cpf = cliente.Cpf;
        entity.Cnpj = cliente.Cnpj;
        entity.CategoriaPerfil = cliente.PerfilCredito.Categoria;
        entity.Bloqueado = cliente.Bloqueado;

        SincronizarContasFiado(cliente, entity);
    }

    private static void SincronizarContasFiado(Cliente cliente, ClienteEntity entity)
    {
        var contasDomain = new List<ContaFiado>();
        if (cliente.ContaFiadoAtiva is not null)
            contasDomain.Add(cliente.ContaFiadoAtiva);

        contasDomain.AddRange(cliente.ContasQuitadas);

        var idsDomain = contasDomain.Select(c => c.Id).ToHashSet();
        var contasRemover = entity.ContasFiado.Where(c => !idsDomain.Contains(c.Id)).ToList();

        foreach (var conta in contasRemover)
            entity.ContasFiado.Remove(conta);

        foreach (var conta in contasDomain)
        {
            var existente = entity.ContasFiado.FirstOrDefault(c => c.Id == conta.Id);

            if (existente is null)
            {
                entity.ContasFiado.Add(ToContaEntity(conta, entity.Id));
                continue;
            }

            existente.DataEmissao = conta.DataEmissao;
            existente.DataVencimento = conta.DataVencimento;
            existente.SaldoDevedor = conta.SaldoDevedor;
            existente.Status = conta.Status;
        }
    }

    private static ContaFiado ToContaDomain(ContaFiadoEntity entity) =>
        ContaFiado.Reconstituir(
            entity.Id,
            entity.DataEmissao,
            entity.DataVencimento,
            entity.SaldoDevedor,
            entity.Status);

    private static ContaFiadoEntity ToContaEntity(ContaFiado conta, Guid clienteId) =>
        new()
        {
            Id = conta.Id,
            ClienteId = clienteId,
            DataEmissao = conta.DataEmissao,
            DataVencimento = conta.DataVencimento,
            SaldoDevedor = conta.SaldoDevedor,
            Status = conta.Status
        };
}
