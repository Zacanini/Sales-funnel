using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWA_Lead_Capture_API.Data;
using PWA_Lead_Capture_API.DTOs;
using PWA_Lead_Capture_API.Extensions;
using PWA_Lead_Capture_API.Models;
using PWA_Lead_Capture_API.Services;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços da aplicação
builder.Services.AddApplicationServices(builder.Configuration);

// Adicionar serviços da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Development");
}
else
{
    app.UseCors("AllowBlazorApp");
}

app.UseHttpsRedirection();

// Garantir que o banco de dados existe
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Endpoints da API

// POST: Criar novo lead
app.MapPost("/api/leads", async (
    [FromBody] LeadDto leadDto,
    AppDbContext context,
    IEmailService emailService,
    ILogger<Program> logger) =>
{
    try
    {
        // Validar dados de entrada
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(leadDto);
        
        if (!Validator.TryValidateObject(leadDto, validationContext, validationResults, true))
        {
            var errors = validationResults.Select(r => r.ErrorMessage).ToArray();
            return Results.BadRequest(new { success = false, errors });
        }

        // Verificar se o email já existe
        var existingLead = await context.Leads
            .FirstOrDefaultAsync(l => l.Email.ToLower() == leadDto.Email.ToLower());

        if (existingLead != null)
        {
            return Results.Conflict(new { 
                success = false, 
                message = "Este email já está cadastrado em nossa base de dados." 
            });
        }

        // Criar novo lead
        var lead = new Lead
        {
            Name = leadDto.Name.Trim(),
            Email = leadDto.Email.Trim().ToLower(),
            Source = leadDto.Source?.Trim(),
            CreatedAt = DateTime.UtcNow,
            Status = "New"
        };

        context.Leads.Add(lead);
        await context.SaveChangesAsync();

        // Tentar enviar email de boas-vindas
        try
        {
            var emailSent = await emailService.SendWelcomeEmailAsync(lead.Email, lead.Name);
            
            if (emailSent)
            {
                lead.EmailSent = true;
                lead.EmailSentAt = DateTime.UtcNow;
                lead.Status = "EmailSent";
                await context.SaveChangesAsync();
                
                logger.LogInformation("Lead criado e email enviado com sucesso: {Email}", lead.Email);
            }
            else
            {
                lead.Status = "EmailFailed";
                await context.SaveChangesAsync();
                
                logger.LogWarning("Lead criado mas falha no envio do email: {Email}", lead.Email);
            }
        }
        catch (Exception emailEx)
        {
            logger.LogError(emailEx, "Erro ao enviar email para o lead: {Email}", lead.Email);
            lead.Status = "EmailError";
            await context.SaveChangesAsync();
        }

        var response = new LeadResponseDto
        {
            Id = lead.Id,
            Name = lead.Name,
            Email = lead.Email,
            CreatedAt = lead.CreatedAt,
            EmailSent = lead.EmailSent,
            Status = lead.Status
        };

        return Results.Ok(new { success = true, data = response });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao criar lead");
        return Results.Problem("Erro interno do servidor");
    }
})
.WithName("CreateLead")
.WithOpenApi();

// GET: Listar leads (para administração)
app.MapGet("/api/leads", async (
    AppDbContext context,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 50,
    [FromQuery] string? status = null) =>
{
    var query = context.Leads.AsQueryable();

    if (!string.IsNullOrEmpty(status))
    {
        query = query.Where(l => l.Status == status);
    }

    var totalLeads = await query.CountAsync();
    
    var leads = await query
        .OrderByDescending(l => l.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(l => new LeadResponseDto
        {
            Id = l.Id,
            Name = l.Name,
            Email = l.Email,
            CreatedAt = l.CreatedAt,
            EmailSent = l.EmailSent,
            Status = l.Status
        })
        .ToListAsync();

    return Results.Ok(new
    {
        success = true,
        data = leads,
        pagination = new
        {
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)totalLeads / pageSize),
            totalLeads
        }
    });
})
.WithName("GetLeads")
.WithOpenApi();

// GET: Buscar lead por ID
app.MapGet("/api/leads/{id:int}", async (int id, AppDbContext context) =>
{
    var lead = await context.Leads.FindAsync(id);

    if (lead == null)
    {
        return Results.NotFound(new { success = false, message = "Lead não encontrado" });
    }

    var response = new LeadResponseDto
    {
        Id = lead.Id,
        Name = lead.Name,
        Email = lead.Email,
        CreatedAt = lead.CreatedAt,
        EmailSent = lead.EmailSent,
        Status = lead.Status
    };

    return Results.Ok(new { success = true, data = response });
})
.WithName("GetLeadById")
.WithOpenApi();

// POST: Reenviar email para um lead
app.MapPost("/api/leads/{id:int}/resend-email", async (
    int id,
    AppDbContext context,
    IEmailService emailService,
    ILogger<Program> logger) =>
{
    var lead = await context.Leads.FindAsync(id);

    if (lead == null)
    {
        return Results.NotFound(new { success = false, message = "Lead não encontrado" });
    }

    try
    {
        var emailSent = await emailService.SendWelcomeEmailAsync(lead.Email, lead.Name);
        
        if (emailSent)
        {
            lead.EmailSent = true;
            lead.EmailSentAt = DateTime.UtcNow;
            lead.Status = "EmailSent";
            await context.SaveChangesAsync();
            
            logger.LogInformation("Email reenviado com sucesso para: {Email}", lead.Email);
            return Results.Ok(new { success = true, message = "Email enviado com sucesso" });
        }
        else
        {
            return Results.Problem("Falha ao enviar email");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao reenviar email para: {Email}", lead.Email);
        return Results.Problem("Erro ao enviar email");
    }
})
.WithName("ResendEmail")
.WithOpenApi();

// GET: Health check
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}))
.WithName("HealthCheck")
.WithOpenApi();

// GET: Estatísticas básicas
app.MapGet("/api/stats", async (AppDbContext context) =>
{
    var totalLeads = await context.Leads.CountAsync();
    var emailsSent = await context.Leads.CountAsync(l => l.EmailSent);
    var todayLeads = await context.Leads.CountAsync(l => l.CreatedAt.Date == DateTime.UtcNow.Date);
    
    var statusDistribution = await context.Leads
        .GroupBy(l => l.Status)
        .Select(g => new { Status = g.Key, Count = g.Count() })
        .ToListAsync();

    return Results.Ok(new
    {
        success = true,
        data = new
        {
            totalLeads,
            emailsSent,
            todayLeads,
            emailSuccessRate = totalLeads > 0 ? Math.Round((double)emailsSent / totalLeads * 100, 2) : 0,
            statusDistribution
        }
    });
})
.WithName("GetStats")
.WithOpenApi();

app.Run();
