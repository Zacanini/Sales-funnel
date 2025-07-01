using Microsoft.EntityFrameworkCore;
using PWA_Lead_Capture_API.Data;
using PWA_Lead_Capture_API.Services;

namespace PWA_Lead_Capture_API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuração do banco de dados SQLite
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Configuração do serviço de email
        services.AddScoped<IEmailService, EmailService>();

        // Configuração CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorApp", policy =>
            {
                policy.WithOrigins(
                    "https://localhost:7163", // URL do Blazor em desenvolvimento
                    "http://localhost:5163",  // URL alternativa do Blazor
                    "https://localhost:5001", // URL padrão HTTPS
                    "http://localhost:5000"   // URL padrão HTTP
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });

            // Política mais permissiva para desenvolvimento
            options.AddPolicy("Development", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }
}
