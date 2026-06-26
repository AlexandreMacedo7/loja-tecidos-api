using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace LojaTecidos.Api.Extensions;

internal static class OpenApiExtensions
{
    public static IServiceCollection AddApiOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>();
            options.AddOperationTransformer<BearerSecuritySchemeOperationTransformer>();
        });

        return services;
    }
}

internal sealed class BearerSecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.AddComponent("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Informe o token JWT obtido em POST /api/auth/login"
        });

        return Task.CompletedTask;
    }
}

internal sealed class BearerSecuritySchemeOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (EndpointPublico(context.Description.RelativePath))
            return Task.CompletedTask;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        });

        return Task.CompletedTask;
    }

    private static bool EndpointPublico(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        return relativePath.Contains("auth/login", StringComparison.OrdinalIgnoreCase)
            || relativePath.Equals("health", StringComparison.OrdinalIgnoreCase);
    }
}
