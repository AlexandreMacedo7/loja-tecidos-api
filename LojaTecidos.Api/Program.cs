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
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<LojaTecidosDbContext>("sqlserver");

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsInDevelopmentAsync();

    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("Loja Tecidos API"));
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true
});

app.MapClienteEndpoints();
app.MapProdutoEndpoints();
app.MapVendaEndpoints();

app.Run();
