# Script de build para Windows PowerShell
# Build e empacotamento do projeto .NET 8 Lambda

Write-Host "=== Build FiapFastFoodAutenticacao Lambda ===" -ForegroundColor Green

# Limpar diretórios anteriores
if (Test-Path "publish") {
    Remove-Item -Recurse -Force "publish"
    Write-Host "Diretório publish removido" -ForegroundColor Yellow
}

if (Test-Path "package.zip") {
    Remove-Item -Force "package.zip"
    Write-Host "package.zip removido" -ForegroundColor Yellow
}

# Restaurar dependências
Write-Host "Restaurando dependências..." -ForegroundColor Blue
dotnet restore ./src/FiapFastFoodAutenticacao/FiapFastFoodAutenticacao.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao restaurar dependências" -ForegroundColor Red
    exit 1
}

# Compilar e publicar
Write-Host "Compilando e publicando..." -ForegroundColor Blue
dotnet publish ./src/FiapFastFoodAutenticacao/FiapFastFoodAutenticacao.csproj -c Release -o ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro na compilação" -ForegroundColor Red
    exit 1
}

# Criar pacote ZIP
Write-Host "Criando pacote ZIP..." -ForegroundColor Blue
Set-Location publish
Compress-Archive -Path * -DestinationPath ../package.zip -Force
Set-Location ..

if (Test-Path "package.zip") {
    $size = (Get-Item "package.zip").Length
    Write-Host "package.zip criado com sucesso! Tamanho: $([math]::Round($size/1MB, 2)) MB" -ForegroundColor Green
} else {
    Write-Host "Erro ao criar package.zip" -ForegroundColor Red
    exit 1
}

Write-Host "=== Build concluído com sucesso! ===" -ForegroundColor Green
Write-Host "Próximos passos:" -ForegroundColor Cyan
Write-Host "1. cd infra/terraform" -ForegroundColor White
Write-Host "2. terraform init" -ForegroundColor White
Write-Host "3. terraform apply" -ForegroundColor White

