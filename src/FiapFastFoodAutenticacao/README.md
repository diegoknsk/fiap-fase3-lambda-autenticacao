# FiapFastFoodAutenticacao - .NET 8 Lambda

Este projeto contém os handlers AWS Lambda para autenticação do sistema FastFood.

## Handlers Disponíveis

### AdminHandler
- **Handler:** `FiapFastFoodAutenticacao::Handlers.AdminHandler::HandleAsync`
- **Função:** Autenticação de administradores (mock do Cognito)
- **Credenciais válidas:**
  - Username: `admin`
  - Password: `fiap@2025`

### Customer Endpoints (Totem)
**IMPORTANTE:** O antigo endpoint de autenticação do totem foi removido. Agora o totem usa os 3 endpoints de Customer:

- **POST** `/api/customer/identify` - Identificar customer por CPF
- **POST** `/api/customer/register` - Registrar novo customer
- **POST** `/api/customer/anonymous` - Registrar customer anônimo

Todos os endpoints retornam `CustomerTokenResponseModel` com JWT token válido por 3 horas.

## Estrutura do Código

### Models
- `AdminLoginRequest` - Request para autenticação admin
- `AdminLoginResponse` - Response da autenticação admin
- `CustomerIdentifyModel` - Request para identificar customer
- `CustomerRegisterModel` - Request para registrar customer
- `CustomerTokenResponseModel` - Response com token JWT para customer
- `Usuario` - Modelo de usuário

### Use Cases
- `IAutenticacaoAdminUseCase` / `AutenticacaoAdminUseCase` - Lógica de negócio para admin
- `CustomerIdentifyUseCase` - Lógica de negócio para identificação de customer
- `CustomerRegisterUseCase` - Lógica de negócio para registro de customer
- `CustomerRegisterAnonymousUseCase` - Lógica de negócio para registro anônimo

### Services
- `ITokenService` / `TokenService` - Geração de tokens JWT
- `IAuthService` / `AuthService` - Serviço de autenticação (admin)

### Repositories
- `IUsuarioRepository` / `UsuarioRepositoryMock` - Acesso a dados de usuários (mock)

### Handlers
- `AdminHandler` - Handler Lambda para autenticação admin
- `CustomerHandler` - Handler Lambda para endpoints de customer (identify, register, anonymous)

### Controllers
- `CustomerController` - Controller para endpoints de customer (identify, register, anonymous)

## Environment Variables

- `SECRET_CONNECTION_STRING_ARN` - ARN do secret no AWS Secrets Manager

## JWT Configuration

O sistema usa JWT para autenticação de customers. As configurações são:

- **Secret:** Configurado via `JwtSettings:Secret` (produção: AWS Secrets Manager)
- **Issuer:** `FiapFastFood`
- **Audience:** `FiapFastFood`
- **Expiration:** 3 horas
- **Subject:** CustomerId (Guid)

## Logs

Os handlers fazem log das operações usando `ILambdaLogger`:
- Informações de início e fim das operações
- Logs de erro em caso de exceções
- Logs de warning se não conseguir ler o secret (não crítico)

## Build Local

```bash
# Restaurar dependências
dotnet restore

# Compilar
dotnet build

# Publicar
dotnet publish -c Release -o ./publish
```

