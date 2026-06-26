using LojaTecidos.Infrastructure.Identity;
using LojaTecidos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LojaTecidos.Api.Extensions;

internal static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        if (app.Environment.IsProduction())
            return;

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LojaTecidosDbContext>();

        if (app.Environment.IsEnvironment("Testing"))
            await context.Database.EnsureCreatedAsync();
        else if (app.Environment.IsDevelopment())
            await context.Database.MigrateAsync();

        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
            await IdentitySeeder.SeedAsync(app.Services);
    }
}
