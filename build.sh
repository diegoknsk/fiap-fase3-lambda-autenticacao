#!/bin/bash
# Script de build para Linux/macOS
# Build e empacotamento das duas Lambdas .NET 8

echo "=== Build FiapFastFoodAutenticacao Lambdas ==="

# Limpar diretórios anteriores
if [ -d "publish" ]; then
    rm -rf publish
    echo "Diretório publish removido"
fi

if [ -f "admin-package.zip" ]; then
    rm admin-package.zip
    echo "admin-package.zip removido"
fi

if [ -f "customer-package.zip" ]; then
    rm customer-package.zip
    echo "customer-package.zip removido"
fi

# Restaurar dependências
echo "Restaurando dependências..."
dotnet restore ./src/FiapFastFoodAutenticacao/FiapFastFoodAutenticacao.csproj
dotnet restore ./src/FiapFastFoodAutenticacao.AdminLambda/FiapFastFoodAutenticacao.AdminLambda.csproj
dotnet restore ./src/FiapFastFoodAutenticacao.CustomerLambda/FiapFastFoodAutenticacao.CustomerLambda.csproj

if [ $? -ne 0 ]; then
    echo "Erro ao restaurar dependências"
    exit 1
fi

# Build Admin Lambda
echo "Compilando e publicando Admin Lambda..."
dotnet publish ./src/FiapFastFoodAutenticacao.AdminLambda/FiapFastFoodAutenticacao.AdminLambda.csproj -c Release -o ./publish/admin

if [ $? -ne 0 ]; then
    echo "Erro na compilação da Admin Lambda"
    exit 1
fi

# Build Customer Lambda
echo "Compilando e publicando Customer Lambda..."
dotnet publish ./src/FiapFastFoodAutenticacao.CustomerLambda/FiapFastFoodAutenticacao.CustomerLambda.csproj -c Release -o ./publish/customer

if [ $? -ne 0 ]; then
    echo "Erro na compilação da Customer Lambda"
    exit 1
fi

# Criar pacotes ZIP
echo "Criando pacotes ZIP..."

# Admin Lambda ZIP
cd publish/admin
zip -r ../../admin-package.zip .
cd ../..

# Customer Lambda ZIP
cd publish/customer
zip -r ../../customer-package.zip .
cd ../..

# Verificar pacotes criados
if [ -f "admin-package.zip" ]; then
    adminSize=$(du -h admin-package.zip | cut -f1)
    echo "admin-package.zip criado com sucesso! Tamanho: $adminSize"
else
    echo "Erro ao criar admin-package.zip"
    exit 1
fi

if [ -f "customer-package.zip" ]; then
    customerSize=$(du -h customer-package.zip | cut -f1)
    echo "customer-package.zip criado com sucesso! Tamanho: $customerSize"
else
    echo "Erro ao criar customer-package.zip"
    exit 1
fi

echo "=== Build concluído com sucesso! ==="
echo "Próximos passos:"
echo "1. cd infra/terraform"
echo "2. terraform init"
echo "3. terraform apply"

