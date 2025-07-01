# Integração Backend + Frontend

## Como integrar a API com o formulário Blazor

### 1. Configurar HttpClient no Frontend

No `Program.cs` do projeto Blazor, adicione:

```csharp
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri("http://localhost:5000") 
});
```

### 2. Criar DTOs no Frontend

```csharp
// Models/LeadDto.cs
public class LeadDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Source { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string[]? Errors { get; set; }
    public string? Message { get; set; }
}
```

### 3. Atualizar FormFields.razor

```csharp
@inject HttpClient Http
@inject NavigationManager NavigationManager

// No método HandleSubmit:
private async Task HandleSubmit()
{
    formError = "";
    
    if (!ValidateForm())
        return;

    isSubmitting = true;
    
    try
    {
        var leadDto = new LeadDto
        {
            Name = formData.Name,
            Email = formData.Email,
            Source = "formulario-cadastro"
        };

        var response = await Http.PostAsJsonAsync("/api/leads", leadDto);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            
            if (result?.Success == true)
            {
                formSuccess = true;
                await OnFormSubmit.InvokeAsync(formData);
            }
            else
            {
                formError = result?.Message ?? "Erro desconhecido";
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            formError = errorResult?.Message ?? "Email já cadastrado";
        }
        else
        {
            formError = "Erro no servidor. Tente novamente.";
        }
    }
    catch (Exception ex)
    {
        formError = "Erro de conexão. Verifique sua internet.";
    }
    finally
    {
        isSubmitting = false;
    }
}
```

### 4. Configurar CORS na API

Certifique-se que a URL do frontend está nas configurações de CORS:

```csharp
// ServiceCollectionExtensions.cs
options.AddPolicy("AllowBlazorApp", policy =>
{
    policy.WithOrigins(
        "https://localhost:7163", // URL do Blazor HTTPS
        "http://localhost:5163"   // URL do Blazor HTTP
    )
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
});
```

### 5. Executar ambos os projetos

#### Terminal 1 - Backend:
```bash
cd PWA-Lead-Capture-API
dotnet run
```

#### Terminal 2 - Frontend:
```bash
cd ..
dotnet run
```

### 6. Fluxo Completo

1. **Usuário preenche formulário** → FormularioCadastro.razor
2. **Submit do formulário** → Dados enviados para API via HTTP POST
3. **API processa** → Validação + Salvamento + Envio de Email
4. **Resposta de sucesso** → Redirecionamento para ConteudoEngajamento.razor
5. **Email enviado** → Usuário recebe email de boas-vindas

### 7. Monitoramento

- **Logs da API**: Console onde a API está rodando
- **Swagger**: http://localhost:5000/swagger
- **Banco SQLite**: Arquivo `leads.db` na pasta da API
- **Estatísticas**: GET http://localhost:5000/api/stats

### 8. Configuração de Email para Produção

Para emails funcionarem em produção, configure:

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "seu-email@gmail.com",
    "SmtpPassword": "sua-senha-de-app-gmail",
    "FromEmail": "noreply@seudominio.com",
    "FromName": "PWA Master"
  }
}
```

**Para Gmail:**
1. Ative autenticação de 2 fatores
2. Gere uma senha de app
3. Use a senha de app no `SmtpPassword`
