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
        var response = await useCase.ExecuteAsync(request, cancellationToken);
        return onSuccess(response);
    }
}
