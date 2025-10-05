# FiapFastFoodAutenticacao - .NET 8 Lambda

Este projeto contém os handlers AWS Lambda para autenticação do sistema FastFood.

## Handlers Disponíveis

### AdminHandler
- **Handler:** `FiapFastFoodAutenticacao::Handlers.AdminHandler::HandleAsync`
- **Função:** Autenticação de administradores (mock do Cognito)
- **Credenciais válidas:**
  - Username: `admin`
  - Password: `fiap@2025`

### TotemHandler
- **Handler:** `FiapFastFoodAutenticacao::Handlers.TotemHandler::HandleAsync`
- **Função:** Autenticação de totens (mock do MySQL)
- **Credenciais válidas:**
  - CPF: `12345678901`
  - Senha: `1234`

## Estrutura do Código

### Models
- `AdminLoginRequest` - Request para autenticação admin
- `AdminLoginResponse` - Response da autenticação admin
- `TotemLoginRequest` - Request para autenticação totem
- `TotemLoginResponse` - Response da autenticação totem
- `Usuario` - Modelo de usuário

### Use Cases
- `IAutenticacaoAdminUseCase` / `AutenticacaoAdminUseCase` - Lógica de negócio para admin
- `IAutenticacaoTotemUseCase` / `AutenticacaoTotemUseCase` - Lógica de negócio para totem

### Repositories
- `IUsuarioRepository` / `UsuarioRepositoryMock` - Acesso a dados de usuários (mock)

### Handlers
- `AdminHandler` - Handler Lambda para autenticação admin
- `TotemHandler` - Handler Lambda para autenticação totem

## Environment Variables

- `SECRET_CONNECTION_STRING_ARN` - ARN do secret no AWS Secrets Manager

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

