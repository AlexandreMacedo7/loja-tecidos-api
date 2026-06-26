using LojaTecidos.Domain.Constants;
using LojaTecidos.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LojaTecidos.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(IdentitySeeder));
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await CriarPapelSeNaoExistirAsync(roleManager, PapeisUsuario.Admin);
        await CriarPapelSeNaoExistirAsync(roleManager, PapeisUsuario.Gerente);

        await CriarUsuarioSeNaoExistirAsync(
            userManager,
            email: "admin@emporiotecidos.com.br",
            senha: "Admin@123",
            nome: "Administrador",
            papel: PapeisUsuario.Admin,
            logger);

        await CriarUsuarioSeNaoExistirAsync(
            userManager,
            email: "gerente@emporiotecidos.com.br",
            senha: "Gerente@123",
            nome: "Gerente da Loja",
            papel: PapeisUsuario.Gerente,
            logger);
    }

    private static async Task CriarPapelSeNaoExistirAsync(RoleManager<IdentityRole> roleManager, string papel)
    {
        if (!await roleManager.RoleExistsAsync(papel))
            await roleManager.CreateAsync(new IdentityRole(papel));
    }

    private static async Task CriarUsuarioSeNaoExistirAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string senha,
        string nome,
        string papel,
        ILogger logger)
    {
        var usuario = await userManager.FindByEmailAsync(email);
        if (usuario is not null)
            return;

        usuario = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Nome = nome,
            EmailConfirmed = true
        };

        var resultado = await userManager.CreateAsync(usuario, senha);
        if (!resultado.Succeeded)
        {
            logger.LogWarning(
                "Falha ao criar usuário seed {Email}: {Erros}",
                email,
                string.Join(", ", resultado.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(usuario, papel);
        logger.LogInformation("Usuário seed {Email} criado com papel {Papel}.", email, papel);
    }
}
