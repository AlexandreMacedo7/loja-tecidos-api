using LojaTecidos.Api.Endpoints;
using LojaTecidos.Api.Extensions;
using LojaTecidos.Application;
using LojaTecidos.Infrastructure;
using LojaTecidos.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddApiOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApiAuth();
builder.Services.AddApiCors(builder.Configuration, builder.Environment);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<LojaTecidosDbContext>("sqlserver");

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("Loja Tecidos API"));
}

app.UseCors("FrontendLocal");

if (!app.Environment.IsEnvironment("Testing"))
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

await app.InitializeDatabaseAsync();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true
});

app.MapAuthEndpoints();
app.MapClienteEndpoints();
app.MapProdutoEndpoints();
app.MapVendaEndpoints();

app.Run();

public partial class Program;
