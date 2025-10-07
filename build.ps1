# Script de build para Windows PowerShell
# Build e empacotamento das duas Lambdas .NET 8

Write-Host "=== Build FiapFastFoodAutenticacao Lambdas ===" -ForegroundColor Green

# Limpar diretórios anteriores
if (Test-Path "publish") {
    Remove-Item -Recurse -Force "publish"
    Write-Host "Diretório publish removido" -ForegroundColor Yellow
}

if (Test-Path "admin-package.zip") {
    Remove-Item -Force "admin-package.zip"
    Write-Host "admin-package.zip removido" -ForegroundColor Yellow
}

if (Test-Path "customer-package.zip") {
    Remove-Item -Force "customer-package.zip"
    Write-Host "customer-package.zip removido" -ForegroundColor Yellow
}

# Restaurar dependências
Write-Host "Restaurando dependências..." -ForegroundColor Blue
dotnet restore ./src/FiapFastFoodAutenticacao/FiapFastFoodAutenticacao.csproj
dotnet restore ./src/FiapFastFoodAutenticacao.AdminLambda/FiapFastFoodAutenticacao.AdminLambda.csproj
dotnet restore ./src/FiapFastFoodAutenticacao.CustomerLambda/FiapFastFoodAutenticacao.CustomerLambda.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao restaurar dependências" -ForegroundColor Red
    exit 1
}

# Build Admin Lambda
Write-Host "Compilando e publicando Admin Lambda..." -ForegroundColor Blue
dotnet publish ./src/FiapFastFoodAutenticacao.AdminLambda/FiapFastFoodAutenticacao.AdminLambda.csproj -c Release -o ./publish/admin

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro na compilação da Admin Lambda" -ForegroundColor Red
    exit 1
}

# Build Customer Lambda
Write-Host "Compilando e publicando Customer Lambda..." -ForegroundColor Blue
dotnet publish ./src/FiapFastFoodAutenticacao.CustomerLambda/FiapFastFoodAutenticacao.CustomerLambda.csproj -c Release -o ./publish/customer

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro na compilação da Customer Lambda" -ForegroundColor Red
    exit 1
}

# Criar pacotes ZIP
Write-Host "Criando pacotes ZIP..." -ForegroundColor Blue

# Admin Lambda ZIP
Set-Location publish/admin
Compress-Archive -Path * -DestinationPath ../../admin-package.zip -Force
Set-Location ../..

# Customer Lambda ZIP
Set-Location publish/customer
Compress-Archive -Path * -DestinationPath ../../customer-package.zip -Force
Set-Location ../..

# Verificar pacotes criados
if (Test-Path "admin-package.zip") {
    $adminSize = (Get-Item "admin-package.zip").Length
    Write-Host "admin-package.zip criado com sucesso! Tamanho: $([math]::Round($adminSize/1MB, 2)) MB" -ForegroundColor Green
} else {
    Write-Host "Erro ao criar admin-package.zip" -ForegroundColor Red
    exit 1
}

if (Test-Path "customer-package.zip") {
    $customerSize = (Get-Item "customer-package.zip").Length
    Write-Host "customer-package.zip criado com sucesso! Tamanho: $([math]::Round($customerSize/1MB, 2)) MB" -ForegroundColor Green
} else {
    Write-Host "Erro ao criar customer-package.zip" -ForegroundColor Red
    exit 1
}

Write-Host "=== Build concluído com sucesso! ===" -ForegroundColor Green
Write-Host "Próximos passos:" -ForegroundColor Cyan
Write-Host "1. cd infra/terraform" -ForegroundColor White
Write-Host "2. terraform init" -ForegroundColor White
Write-Host "3. terraform apply" -ForegroundColor White

