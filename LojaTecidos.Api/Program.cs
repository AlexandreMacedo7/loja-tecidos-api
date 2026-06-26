using LojaTecidos.Api.Endpoints;
using LojaTecidos.Application;
using LojaTecidos.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapClienteEndpoints();
app.MapProdutoEndpoints();
app.MapVendaEndpoints();

app.Run();
