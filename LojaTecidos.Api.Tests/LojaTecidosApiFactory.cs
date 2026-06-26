using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace LojaTecidos.Api.Tests;

public sealed class LojaTecidosApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"LojaTecidosTests_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Testing",
                ["Testing:InMemoryDatabaseName"] = _databaseName,
                ["Jwt:Issuer"] = "LojaTecidos.Test",
                ["Jwt:Audience"] = "LojaTecidos.Api.Test",
                ["Jwt:ChaveSecreta"] = "TestSecretKeyWithAtLeast32Characters!",
                ["Jwt:ExpiracaoHoras"] = "1"
            });
        });
    }
}

[CollectionDefinition("Api")]
public sealed class ApiCollection : ICollectionFixture<LojaTecidosApiFactory>;
