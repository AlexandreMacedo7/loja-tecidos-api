using LojaTecidos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LojaTecidos.Api.Extensions;

internal static class DatabaseExtensions
{
    public static async Task ApplyMigrationsInDevelopmentAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LojaTecidosDbContext>();
        await context.Database.MigrateAsync();
    }
}
