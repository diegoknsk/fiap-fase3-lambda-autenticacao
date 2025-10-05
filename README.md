# FiapFastFoodAutenticacao - Clean Architecture + AWS Lambda .NET 8

Solução com Clean Architecture, fonte única de lógica de negócio, e deploy serverless no AWS Lambda.

## 🏗️ Arquitetura

```
┌─────────────────────────────────────────────────────────────┐
│                    FiapFastFoodAutenticacao                 │
│                    (Core/App - Fonte Única)                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   Contracts/    │  │     DTOs/       │  │  Services/   │ │
│  │  IAuthService   │  │ AuthRequests    │  │ AuthService  │ │
│  │                 │  │ AuthResponses   │  │              │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ (referencia)
                              ▼
┌─────────────────────────────────────────────────────────────┐
│              FiapFastFoodAutenticacao.DebugApi              │
│              (Host de Depuração - Minimal API)              │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │  Program.cs - Apenas roteamento e delegação            │ │
│  │  POST /autenticacaoAdmin → IAuthService                │ │
│  │  POST /autenticacaoTotem → IAuthService                │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ (deploy)
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    AWS Lambda Function                      │
│              (LambdaEntryPoint + Startup)                   │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │  Mesmos endpoints, mesma lógica, ambiente produção     │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## 📁 Estrutura do Projeto

```
/src
  /FiapFastFoodAutenticacao            <-- Core/App: UseCases + Services + Contracts (SEM dependência de Web)
    /Contracts
      IAuthService.cs
    /Dtos
      AuthRequests.cs
      AuthResponses.cs
    /Services
      AuthService.cs
    LambdaEntryPoint.cs
    Startup.cs
    Program.cs

  /FiapFastFoodAutenticacao.DebugApi   <-- Host de depuração (Minimal API)
    Program.cs                         <-- Apenas roteamento, delega para Core

  /FiapFastFoodAutenticacao.Tests      <-- Unit tests do Core/App
    Program.cs

/infra/terraform                        <-- IaC do Lambda
  main.tf
  variables.tf
  outputs.tf
  providers.tf

.github/workflows/                      <-- Deploy automático
  publish-lambda.yml
```

## 🎯 Princípios

- ✅ **Fonte única de lógica** - Toda regra de negócio no projeto Core
- ✅ **DebugApi é apenas host** - Nenhuma regra de negócio no DebugApi
- ✅ **Deploy apenas Lambda** - DebugApi não vai para produção
- ✅ **Secrets via AWS Secrets Manager** - Nada sensível no repo
- ✅ **Clean Architecture simples** - Sem DI pesado entre camadas

## 🚀 Como Usar

### 1. Debug Local (Desenvolvimento)
```bash
# Executar API de debug com Swagger
cd src/FiapFastFoodAutenticacao.DebugApi
dotnet run

# Acessar: http://localhost:5000
# Testar endpoints:
# POST /autenticacaoAdmin
# POST /autenticacaoTotem
```

### 2. Testes Unitários
```bash
# Executar testes do Core
cd src/FiapFastFoodAutenticacao.Tests
dotnet run
```

### 3. Deploy para Produção
```bash
# Deploy automático via GitHub Actions
git add .
git commit -m "feat: implementar autenticação"
git push origin main

# Ou deploy manual via Terraform
cd infra/terraform
terraform init
terraform plan
terraform apply
```

## 📋 Credenciais de Teste

### Admin
- **Email:** `admin@fiap.com`
- **Password:** `fiap@2025`

### Totem
- **CPF:** `12345678901`

## 🔐 Secrets

- **Secret Name:** `fastfood/db/connection-string`
- **Acesso:** AWS Secrets Manager
- **Policy:** `secretsmanager:GetSecretValue`

## Pré-requisitos

- .NET 8 SDK
- AWS CLI configurado
- Terraform >= 1.7.0
- Conta AWS com permissões para Lambda, IAM e Secrets Manager

## Build e Empacotamento

### 1. Restaurar dependências e compilar

```bash
# Restaurar dependências
dotnet restore ./src/FiapFastFoodAutenticacao/FiapFastFoodAutenticacao.csproj

# Compilar e publicar
dotnet publish ./src/FiapFastFoodAutenticacao/FiapFastFoodAutenticacao.csproj -c Release -o ./publish

# Criar pacote ZIP
cd publish
zip -r ../package.zip .
cd ..
```

### 2. Verificar se o package.zip foi criado

```bash
ls -la package.zip
```

## Deploy com Terraform

### 1. Configurar variáveis (opcional)

```bash
cd infra/terraform
cp secrets.auto.tfvars.example secrets.auto.tfvars
# Edite secrets.auto.tfvars se necessário
```

### 2. Inicializar e aplicar Terraform

```bash
# Inicializar Terraform
terraform init

# Planejar deploy
terraform plan

# Aplicar infraestrutura
terraform apply
```

### 3. Verificar outputs

```bash
terraform output
```

## Testando os Lambdas

### Testar Admin Handler

```bash
aws lambda invoke \
  --function-name FiapFastFoodAutenticacao-Admin \
  --payload '{ "Username":"admin", "Password":"fiap@2025" }' \
  out-admin.json --cli-binary-format raw-in-base64-out

type out-admin.json
```

**Resposta esperada:**
```json
{
  "success": true,
  "token": "MOCK_ADMIN_JWT",
  "message": "ok"
}
```

### Testar Totem Handler

```bash
aws lambda invoke \
  --function-name FiapFastFoodAutenticacao-Totem \
  --payload '{ "Cpf":"12345678901", "Senha":"1234" }' \
  out-totem.json --cli-binary-format raw-in-base64-out

type out-totem.json
```

**Resposta esperada:**
```json
{
  "success": true,
  "token": "MOCK_TOTEM_JWT",
  "message": "ok",
  "usuario": {
    "id": "1",
    "nome": "Cliente Teste",
    "cpf": "12345678901"
  }
}
```

### Testar credenciais inválidas

```bash
# Admin com credenciais inválidas
aws lambda invoke \
  --function-name FiapFastFoodAutenticacao-Admin \
  --payload '{ "Username":"admin", "Password":"wrong" }' \
  out-admin-error.json --cli-binary-format raw-in-base64-out

type out-admin-error.json

# Totem com senha inválida
aws lambda invoke \
  --function-name FiapFastFoodAutenticacao-Totem \
  --payload '{ "Cpf":"12345678901", "Senha":"wrong" }' \
  out-totem-error.json --cli-binary-format raw-in-base64-out

type out-totem-error.json
```

## Recursos Criados

### AWS Lambda Functions
- `FiapFastFoodAutenticacao-Admin` - Handler para autenticação de administradores
- `FiapFastFoodAutenticacao-Totem` - Handler para autenticação de totens

### AWS Secrets Manager
- `fastfood/db/connection-string` - Secret com connection string do MySQL (mock)

### IAM
- Role `FiapFastFoodAutenticacao-lambda-exec-role` com permissões para:
  - Execução básica do Lambda
  - Leitura de secrets do AWS Secrets Manager

## Configurações Mock

### Admin Authentication
- **Username:** `admin`
- **Password:** `fiap@2025`
- **Token retornado:** `MOCK_ADMIN_JWT`

### Totem Authentication
- **CPF:** `12345678901`
- **Senha:** `1234`
- **Usuário mock:** `Cliente Teste`
- **Token retornado:** `MOCK_TOTEM_JWT`

## Logs

Os logs podem ser visualizados no CloudWatch Logs:
- `/aws/lambda/FiapFastFoodAutenticacao-Admin`
- `/aws/lambda/FiapFastFoodAutenticacao-Totem`

## Limpeza

Para remover todos os recursos criados:

```bash
cd infra/terraform
terraform destroy
```

## Arquitetura

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   AWS Lambda    │    │   Use Cases      │    │   Repository    │
│   Handlers      │───▶│   (Business      │───▶│   (Mock Data)   │
│                 │    │    Logic)        │    │                 │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                       │
         ▼                       ▼
┌─────────────────┐    ┌──────────────────┐
│   AWS Secrets   │    │   Environment    │
│   Manager       │    │   Variables      │
└─────────────────┘    └──────────────────┘
```

## Tecnologias Utilizadas

- **.NET 8** - Runtime do Lambda
- **AWS Lambda** - Serverless compute
- **AWS Secrets Manager** - Gerenciamento de secrets
- **Terraform** - Infrastructure as Code
- **AWS IAM** - Controle de acesso
Repositório para lambda de autenticação da AWS
