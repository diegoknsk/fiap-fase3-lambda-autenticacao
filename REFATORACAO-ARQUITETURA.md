# Refatoração da Arquitetura - Duas Lambdas Separadas

## Visão Geral

O projeto foi refatorado para usar duas Lambdas separadas com API Gateway HTTP API v2, seguindo as melhores práticas de arquitetura serverless.

## Nova Arquitetura

### Lambda 1: FastFoodAutenticacaoAdmin
- **Localização**: `src/FiapFastFoodAutenticacao.AdminLambda/`
- **Função**: Login de administradores via Cognito
- **VPC**: Não (acessa Cognito externamente)
- **Handler**: `AdminHandler`

### Lambda 2: FastFoodAutenticacaoCustomer
- **Localização**: `src/FiapFastFoodAutenticacao.CustomerLambda/`
- **Função**: Operações de customer (identify/register/anonymous)
- **VPC**: Sim (acessa RDS)
- **Handler**: `CustomerHandler`

### API Gateway HTTP API v2
- **Nome**: `fastfood-http`
- **Rotas**:
  - `POST /admin/login` → FastFoodAutenticacaoAdmin
  - `POST /customer/identify` → FastFoodAutenticacaoCustomer

## Estrutura de Arquivos

```
src/
├── FiapFastFoodAutenticacao/                    # Projeto base (handlers, models, etc.)
├── FiapFastFoodAutenticacao.AdminLambda/        # Lambda Admin
│   ├── Function.cs
│   └── FiapFastFoodAutenticacao.AdminLambda.csproj
└── FiapFastFoodAutenticacao.CustomerLambda/     # Lambda Customer
    ├── Function.cs
    └── FiapFastFoodAutenticacao.CustomerLambda.csproj
```

## Terraform Backend S3

```hcl
terraform {
  backend "s3" {
    bucket = "fiap-fase3-tfstate"
    key    = "infra-lambda/terraform.tfstate"
    region = "us-east-1"
  }
}
```

## Build e Deploy

### 1. Build das Lambdas
```bash
# Windows
.\build.ps1

# Linux/macOS
./build.sh
```

Isso criará:
- `admin-package.zip` - Pacote da Admin Lambda
- `customer-package.zip` - Pacote da Customer Lambda

### 2. Deploy via Terraform
```bash
cd infra/terraform
terraform init
terraform apply
```

## Rotas da API

### POST /admin/login
- **Lambda**: FastFoodAutenticacaoAdmin
- **Body**: `AdminLoginRequest`
- **Response**: `AdminLoginResponse`

### POST /customer/identify
- **Lambda**: FastFoodAutenticacaoCustomer
- **Body**: `CustomerIdentifyModel`
- **Response**: `CustomerTokenResponseModel`

## Extensibilidade

A arquitetura está preparada para adicionar facilmente novas rotas:

```csharp
// Em CustomerLambda/Function.cs
switch (path)
{
    case "/customer/identify":
        // Já implementado
        break;
    case "/customer/register":
        // Fácil de adicionar
        break;
    case "/customer/anonymous":
        // Fácil de adicionar
        break;
}
```

## Vantagens da Nova Arquitetura

1. **Separação de Responsabilidades**: Cada Lambda tem uma função específica
2. **Otimização de Recursos**: Admin Lambda sem VPC (mais rápido)
3. **Escalabilidade**: Cada Lambda escala independentemente
4. **Manutenibilidade**: Código mais organizado e fácil de manter
5. **Segurança**: Customer Lambda isolada em VPC para acesso ao RDS
6. **Performance**: HTTP API v2 é mais rápido que REST API

## Configuração de Ambiente

### Admin Lambda
- Sem VPC
- Acesso direto ao Cognito
- Variáveis de ambiente mínimas

### Customer Lambda
- Com VPC
- Acesso ao RDS
- Configuração JWT
- Connection string do banco

## Próximos Passos

1. Testar o build local
2. Fazer deploy via Terraform
3. Testar as rotas da API
4. Adicionar novas rotas conforme necessário
5. Configurar monitoramento e logs
