using LojaTecidos.Application.Abstractions;

namespace LojaTecidos.Api.Extensions;

internal static class UseCaseEndpointExtensions
{
    public static async Task<IResult> ExecuteAsync<TRequest, TResponse>(
        IUseCase<TRequest, TResponse> useCase,
        TRequest request,
        Func<TResponse, IResult> onSuccess,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await useCase.ExecuteAsync(request, cancellationToken);
            return onSuccess(response);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message.Contains("não encontrad", StringComparison.OrdinalIgnoreCase)
                ? Results.NotFound(new { erro = ex.Message })
                : Results.BadRequest(new { erro = ex.Message });
        }
    }
}
