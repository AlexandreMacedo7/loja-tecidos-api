using System.Diagnostics;
using LojaTecidos.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaTecidos.Api.Extensions;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IHostEnvironment environment, ILogger<GlobalExceptionHandler> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, titulo, detalhe) = MapearExcecao(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Erro não tratado ao processar {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        else
            _logger.LogWarning(exception, "Requisição rejeitada: {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = titulo,
            Detail = detalhe,
            Instance = httpContext.Request.Path
        };

        if (_environment.IsDevelopment())
            problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Titulo, string Detalhe) MapearExcecao(Exception exception)
    {
        return exception switch
        {
            EntidadeNaoEncontradaException ex => (
                StatusCodes.Status404NotFound,
                "Recurso não encontrado",
                ex.Message),
            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                "Requisição inválida",
                ex.Message),
            RegraNegocioException ex => (
                StatusCodes.Status400BadRequest,
                "Regra de negócio violada",
                ex.Message),
            InvalidOperationException ex => (
                StatusCodes.Status400BadRequest,
                "Operação inválida",
                ex.Message),
            DbUpdateException ex => (
                StatusCodes.Status400BadRequest,
                "Erro ao persistir dados",
                ObterMensagemDbUpdate(ex)),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro interno do servidor",
                "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };
    }

    private static string ObterMensagemDbUpdate(DbUpdateException exception)
    {
        var inner = exception.InnerException?.Message ?? exception.Message;

        if (inner.Contains("truncated", StringComparison.OrdinalIgnoreCase))
            return "Um ou mais campos excedem o tamanho permitido. Verifique CPF, CNPJ e demais textos enviados.";

        if (inner.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
            inner.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
            return "Registro duplicado. Verifique códigos ou identificadores únicos.";

        return "Não foi possível salvar os dados informados.";
    }
}
