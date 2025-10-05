# Script para atualizar credenciais AWS Academy no GitHub
# Execute este script quando suas credenciais expirarem

param(
    [Parameter(Mandatory=$true)]
    [string]$AccessKeyId,
    
    [Parameter(Mandatory=$true)]
    [string]$SecretAccessKey,
    
    [Parameter(Mandatory=$true)]
    [string]$SessionToken,
    
    [Parameter(Mandatory=$false)]
    [string]$Repository = "fiap-fase3-lambda-autenticacao"
)

Write-Host "=== Atualizando Credenciais AWS Academy no GitHub ===" -ForegroundColor Green

# Verificar se gh CLI está instalado
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "❌ GitHub CLI (gh) não encontrado. Instale em: https://cli.github.com/" -ForegroundColor Red
    exit 1
}

# Verificar se está logado no GitHub
try {
    gh auth status
} catch {
    Write-Host "❌ Não está logado no GitHub. Execute: gh auth login" -ForegroundColor Red
    exit 1
}

Write-Host "🔄 Atualizando secrets no GitHub..." -ForegroundColor Blue

# Atualizar AWS_ACCESS_KEY_ID
Write-Host "Atualizando AWS_ACCESS_KEY_ID..." -ForegroundColor Yellow
gh secret set AWS_ACCESS_KEY_ID --body $AccessKeyId --repo $Repository

# Atualizar AWS_SECRET_ACCESS_KEY
Write-Host "Atualizando AWS_SECRET_ACCESS_KEY..." -ForegroundColor Yellow
gh secret set AWS_SECRET_ACCESS_KEY --body $SecretAccessKey --repo $Repository

# Atualizar AWS_SESSION_TOKEN
Write-Host "Atualizando AWS_SESSION_TOKEN..." -ForegroundColor Yellow
gh secret set AWS_SESSION_TOKEN --body $SessionToken --repo $Repository

Write-Host "✅ Credenciais atualizadas com sucesso!" -ForegroundColor Green
Write-Host "🚀 Agora você pode fazer push para deploy:" -ForegroundColor Cyan
Write-Host "   git push origin dev" -ForegroundColor White
