using LojaTecidos.Domain.Entities.Enum;

namespace LojaTecidos.Application.Common.Dtos;

public sealed record EnderecoDto(string Rua, string Numero, string Bairro, string Cep);

public sealed record ContaFiadoDto(
    Guid Id,
    DateTime DataEmissao,
    DateTime DataVencimento,
    decimal SaldoDevedor,
    StatusContaFiado Status);

public sealed record ClienteDto(
    Guid Id,
    string Nome,
    string Telefone,
    EnderecoDto Endereco,
    string? Cpf,
    string? Cnpj,
    CategoriaPerfil CategoriaPerfil,
    decimal LimiteCredito,
    bool Bloqueado,
    ContaFiadoDto? ContaFiadoAtiva);

public sealed record ProdutoDto(
    Guid Id,
    string CodigoInterno,
    string? CodigoFornecedor,
    string Nome,
    string Fornecedor,
    CategoriaProduto Categoria,
    UnidadeMedida UnidadeMedida,
    decimal PrecoUnitario,
    bool Descontinuado,
    decimal QuantidadeAtual,
    decimal QuantidadeReferenciaAlerta,
    bool ProximoDoFim,
    bool Parado);

public sealed record ItemVendaDto(
    string CodigoInternoProduto,
    decimal Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal);

public sealed record VendaDto(
    Guid Id,
    int NumeroSequencial,
    string CodigoVenda,
    DateTime DataVenda,
    TipoVenda Tipo,
    StatusVenda Status,
    decimal DescontoPercentual,
    decimal TotalBruto,
    decimal ValorDesconto,
    decimal TotalLiquido,
    string? UsuarioId,
    IReadOnlyList<ItemVendaDto> Itens);

public sealed record AlertaEstoqueDto(
    string CodigoInterno,
    string Nome,
    decimal QuantidadeAtual,
    decimal QuantidadeReferenciaAlerta,
    bool ProximoDoFim,
    bool Parado);

public sealed record TokenAutenticacaoDto(
    string Token,
    string Email,
    string Nome,
    IReadOnlyList<string> Papeis,
    DateTime ExpiraEm);
