# Teste da API PWA Lead Capture

## Base URL
```
http://localhost:5000
```

## 1. Health Check
```http
GET /health
```

## 2. Criar Lead
```http
POST /api/leads
Content-Type: application/json

{
  "name": "João Silva",
  "email": "joao.silva@email.com",
  "source": "landing-page"
}
```

## 3. Listar Leads
```http
GET /api/leads?page=1&pageSize=10
```

## 4. Buscar Lead por ID
```http
GET /api/leads/1
```

## 5. Reenviar Email
```http
POST /api/leads/1/resend-email
```

## 6. Estatísticas
```http
GET /api/stats
```

## Teste com cURL

### Criar Lead
```bash
curl -X POST "http://localhost:5000/api/leads" \
     -H "Content-Type: application/json" \
     -d '{
       "name": "Maria Santos",
       "email": "maria.santos@email.com",
       "source": "formulario-cadastro"
     }'
```

### Health Check
```bash
curl -X GET "http://localhost:5000/health"
```

### Listar Leads
```bash
curl -X GET "http://localhost:5000/api/leads"
```

### Estatísticas
```bash
curl -X GET "http://localhost:5000/api/stats"
```

## Exemplos de Resposta

### Sucesso ao Criar Lead
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "João Silva",
    "email": "joao.silva@email.com",
    "createdAt": "2025-01-20T10:00:00Z",
    "emailSent": true,
    "status": "EmailSent"
  }
}
```

### Erro de Validação
```json
{
  "success": false,
  "errors": [
    "Nome é obrigatório",
    "Email deve ter um formato válido"
  ]
}
```

### Email Duplicado
```json
{
  "success": false,
  "message": "Este email já está cadastrado em nossa base de dados."
}
```
