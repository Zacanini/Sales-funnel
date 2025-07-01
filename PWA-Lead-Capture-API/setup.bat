@echo off
echo ============================================
echo   PWA Lead Capture API - Setup Script
echo ============================================
echo.

echo [1/5] Verificando .NET 8...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERRO: .NET 8 nao encontrado. Instale em https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo ✓ .NET encontrado

echo.
echo [2/5] Restaurando pacotes NuGet...
dotnet restore
if %errorlevel% neq 0 (
    echo ERRO: Falha ao restaurar pacotes
    pause
    exit /b 1
)
echo ✓ Pacotes restaurados

echo.
echo [3/5] Compilando projeto...
dotnet build
if %errorlevel% neq 0 (
    echo ERRO: Falha na compilacao
    pause
    exit /b 1
)
echo ✓ Projeto compilado

echo.
echo [4/5] Verificando configurações de email...
if not exist "appsettings.Development.json" (
    echo AVISO: Configure as credenciais de email em appsettings.Development.json
    echo Exemplo para Gmail:
    echo {
    echo   "EmailSettings": {
    echo     "SmtpUsername": "seu-email@gmail.com",
    echo     "SmtpPassword": "sua-senha-de-app"
    echo   }
    echo }
)
echo ✓ Configurações verificadas

echo.
echo [5/5] Tudo pronto! Executando API...
echo.
echo API estará disponível em:
echo - HTTP:    http://localhost:5000
echo - Swagger: http://localhost:5000/swagger
echo.
echo Pressione Ctrl+C para parar o servidor
echo.

dotnet run
