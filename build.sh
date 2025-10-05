#!/bin/bash
# Script de build para Linux/macOS
# Build e empacotamento do projeto .NET 8 Lambda

echo "=== Build FiapFastFoodAutenticacao Lambda ==="

# Limpar diretórios anteriores
if [ -d "publish" ]; then
    rm -rf publish
    echo "Diretório publish removido"
fi

if [ -f "package.zip" ]; then
    rm package.zip
    echo "package.zip removido"
fi

# Restaurar dependências
echo "Restaurando dependências..."
dotnet restore ./src/FiapFastFoodAutenticacao/FiapFastFoodAutenticacao.csproj

if [ $? -ne 0 ]; then
    echo "Erro ao restaurar dependências"
    exit 1
fi

# Compilar e publicar
echo "Compilando e publicando..."
dotnet publish ./src/FiapFastFoodAutenticacao/FiapFastFoodAutenticacao.csproj -c Release -o ./publish

if [ $? -ne 0 ]; then
    echo "Erro na compilação"
    exit 1
fi

# Criar pacote ZIP
echo "Criando pacote ZIP..."
cd publish
zip -r ../package.zip .
cd ..

if [ -f "package.zip" ]; then
    size=$(du -h package.zip | cut -f1)
    echo "package.zip criado com sucesso! Tamanho: $size"
else
    echo "Erro ao criar package.zip"
    exit 1
fi

echo "=== Build concluído com sucesso! ==="
echo "Próximos passos:"
echo "1. cd infra/terraform"
echo "2. terraform init"
echo "3. terraform apply"

