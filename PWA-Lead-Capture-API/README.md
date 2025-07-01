# PWA Lead Capture API

API backend para capturar leads do funil de vendas PWA Master, desenvolvida em .NET 8 Minimal API com SQLite e envio de emails.

## 🚀 Funcionalidades

- ✅ Captura de dados de leads (nome, email, source)
- ✅ Armazenamento em banco SQLite
- ✅ Envio automático de email de boas-vindas
- ✅ Validação de dados
- ✅ Prevenção de emails duplicados
- ✅ API RESTful completa
- ✅ Documentação Swagger
- ✅ CORS configurado para Blazor
- ✅ Logs estruturados
- ✅ Health checks

## 🛠️ Tecnologias

- .NET 8
- Entity Framework Core
- SQLite
- MailKit (envio de emails)
- Swagger/OpenAPI

## 📋 Pré-requisitos

- .NET 8 SDK
- Uma conta de email para SMTP (Gmail recomendado)

## ⚙️ Configuração

### 1. Instalação rápida (Windows)

Execute o script de setup:
```bash
setup.bat
```

### 2. Instalação manual

```bash
cd PWA-Lead-Capture-API
dotnet restore
dotnet build
```

### 3. Configure o email (appsettings.Development.json)

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "seu-email@gmail.com",
    "SmtpPassword": "sua-senha-de-app",
    "FromEmail": "noreply@pwamaster.com",
    "FromName": "PWA Master"
  }
}
```

**Para Gmail:**
1. Ative a verificação em 2 etapas
2. Gere uma senha de app específica
3. Use a senha de app no campo `SmtpPassword`

### 3. Execute a aplicação

```bash
dotnet run
```

A API estará disponível em:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: https://localhost:5001/swagger

## 📊 Endpoints da API

### POST /api/leads
Cria um novo lead e envia email de boas-vindas.

**Body:**
```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "source": "landing-page"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "João Silva",
    "email": "joao@email.com",
    "createdAt": "2025-01-20T10:00:00Z",
    "emailSent": true,
    "status": "EmailSent"
  }
}
```

### GET /api/leads
Lista todos os leads (paginado).

**Query Parameters:**
- `page` (default: 1)
- `pageSize` (default: 50)
- `status` (opcional: "New", "EmailSent", "EmailFailed")

### GET /api/leads/{id}
Busca um lead específico por ID.

### POST /api/leads/{id}/resend-email
Reenvia o email de boas-vindas para um lead.

### GET /api/stats
Retorna estatísticas gerais dos leads.

### GET /health
Health check da aplicação.

## 📁 Estrutura do Projeto

```
PWA-Lead-Capture-API/
├── Models/
│   └── Lead.cs                 # Modelo de dados
├── DTOs/
│   └── LeadDto.cs             # Data Transfer Objects
├── Data/
│   └── AppDbContext.cs        # Contexto do Entity Framework
├── Services/
│   ├── IEmailService.cs       # Interface do serviço de email
│   └── EmailService.cs        # Implementação do serviço de email
├── Extensions/
│   └── ServiceCollectionExtensions.cs  # Configuração de serviços
├── Program.cs                 # Ponto de entrada e configuração da API
├── appsettings.json          # Configurações gerais
├── appsettings.Development.json  # Configurações de desenvolvimento
└── leads.db                  # Banco SQLite (criado automaticamente)
```

## 🔒 Segurança

- Validação rigorosa de dados de entrada
- Prevenção de emails duplicados
- CORS configurado especificamente para o frontend Blazor
- Logs de auditoria para todas as operações

## 📧 Template de Email

O email de boas-vindas inclui:
- Design responsivo em HTML
- Versão texto alternativa
- Links para conteúdo exclusivo
- Instruções de próximos passos
- Branding do PWA Master

## 🚀 Deploy

### Desenvolvimento
```bash
dotnet run --environment Development
```

### Produção
```bash
dotnet publish -c Release
```

## 🔍 Monitoramento

- Logs estruturados com diferentes níveis
- Health check endpoint
- Estatísticas de conversão de email
- Tracking de status dos leads

## 🤝 Integração com Frontend

### Exemplo de integração com Blazor WebAssembly:

```csharp
@inject HttpClient Http

// Enviar dados do formulário
public async Task SubmitForm(FormData data)
{
    var leadDto = new
    {
        name = data.Name,
        email = data.Email,
        source = "formulario-cadastro"
    };

    var response = await Http.PostAsJsonAsync("http://localhost:5000/api/leads", leadDto);
    
    if (response.IsSuccessStatusCode)
    {
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        // Sucesso - redirecionar para página de agradecimento
    }
    else
    {
        // Tratar erro
        var error = await response.Content.ReadAsStringAsync();
    }
}
```

### Exemplo com JavaScript/TypeScript:

```javascript
async function submitLead(name, email, source = 'landing-page') {
    try {
        const response = await fetch('http://localhost:5000/api/leads', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                name: name,
                email: email,
                source: source
            })
        });

        const result = await response.json();
        
        if (result.success) {
            console.log('Lead criado:', result.data);
            // Redirecionar para página de sucesso
        } else {
            console.error('Erro:', result.errors || result.message);
        }
    } catch (error) {
        console.error('Erro de rede:', error);
    }
}
```

A API está configurada para funcionar perfeitamente com o frontend Blazor WebAssembly. Certifique-se de que as URLs de CORS no `ServiceCollectionExtensions.cs` correspondam às URLs do seu frontend.

## 📝 Notas Importantes

1. **Banco de Dados**: O SQLite será criado automaticamente na primeira execução
2. **Emails**: Configure as credenciais SMTP antes de usar em produção
3. **CORS**: Ajuste as URLs permitidas conforme seu ambiente
4. **Logs**: Por padrão, os logs são exibidos no console

## 🐛 Troubleshooting

### Email não enviado
- Verifique as credenciais SMTP
- Confirme que a conta tem autenticação de 2 fatores ativada (Gmail)
- Use senha de app específica (não a senha da conta)

### Erro de CORS
- Verifique se a URL do frontend está na lista de origens permitidas
- Confirme se o frontend está rodando na porta esperada

### Banco de dados
- O SQLite é criado automaticamente
- Se houver problemas, delete o arquivo `leads.db` e reinicie a aplicação
