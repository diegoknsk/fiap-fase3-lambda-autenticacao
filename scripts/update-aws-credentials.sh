#!/bin/bash

# Script para atualizar credenciais AWS Academy no GitHub
# Execute este script quando suas credenciais expirarem

# Verificar parâmetros
if [ $# -lt 3 ]; then
    echo "❌ Uso: $0 <ACCESS_KEY_ID> <SECRET_ACCESS_KEY> <SESSION_TOKEN> [REPOSITORY]"
    echo "   Exemplo: $0 ASIAIOSFODNN7EXAMPLE wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY IQoJb3JpZ2luX2VjE..."
    exit 1
fi

ACCESS_KEY_ID=$1
SECRET_ACCESS_KEY=$2
SESSION_TOKEN=$3
REPOSITORY=${4:-"fiap-fase3-lambda-autenticacao"}

echo "=== Atualizando Credenciais AWS Academy no GitHub ==="

# Verificar se gh CLI está instalado
if ! command -v gh &> /dev/null; then
    echo "❌ GitHub CLI (gh) não encontrado. Instale em: https://cli.github.com/"
    exit 1
fi

# Verificar se está logado no GitHub
if ! gh auth status &> /dev/null; then
    echo "❌ Não está logado no GitHub. Execute: gh auth login"
    exit 1
fi

echo "🔄 Atualizando secrets no GitHub..."

# Atualizar AWS_ACCESS_KEY_ID
echo "Atualizando AWS_ACCESS_KEY_ID..."
gh secret set AWS_ACCESS_KEY_ID --body "$ACCESS_KEY_ID" --repo "$REPOSITORY"

# Atualizar AWS_SECRET_ACCESS_KEY
echo "Atualizando AWS_SECRET_ACCESS_KEY..."
gh secret set AWS_SECRET_ACCESS_KEY --body "$SECRET_ACCESS_KEY" --repo "$REPOSITORY"

# Atualizar AWS_SESSION_TOKEN
echo "Atualizando AWS_SESSION_TOKEN..."
gh secret set AWS_SESSION_TOKEN --body "$SESSION_TOKEN" --repo "$REPOSITORY"

echo "✅ Credenciais atualizadas com sucesso!"
echo "🚀 Agora você pode fazer push para deploy:"
echo "   git push origin main"
