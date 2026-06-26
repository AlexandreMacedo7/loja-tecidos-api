using System.Text;
using LojaTecidos.Api.Authorization;
using LojaTecidos.Domain.Constants;
using LojaTecidos.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LojaTecidos.Api.Extensions;

internal static class AuthExtensions
{
    public static IServiceCollection AddApiAuth(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtSettings) =>
            {
                var settings = jwtSettings.Value;

                if (string.IsNullOrWhiteSpace(settings.ChaveSecreta))
                    throw new InvalidOperationException("Configuração JWT: ChaveSecreta não informada.");

                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(settings.ChaveSecreta)),
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PoliticasAutorizacao.Admin, policy =>
                policy.RequireRole(PapeisUsuario.Admin));

            options.AddPolicy(PoliticasAutorizacao.GerenteOuAdmin, policy =>
                policy.RequireRole(PapeisUsuario.Admin, PapeisUsuario.Gerente));
        });

        return services;
    }

    public static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var origens = configuration.GetSection("Cors:Origens").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("FrontendLocal", policy =>
            {
                if (environment.IsDevelopment() && origens.Length == 0)
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    return;
                }

                policy
                    .WithOrigins(origens)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
