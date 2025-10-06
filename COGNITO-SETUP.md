# Configuração do Cognito - FastFood Authentication

## Resumo das Alterações

Este documento descreve as alterações implementadas para integrar o AWS Cognito ao sistema de autenticação.

### 1. Arquivos Criados/Modificados

#### Novos Arquivos:
- `infra/terraform/cognito.tf` - Configuração do Cognito User Pool, App Client e usuário admin1

#### Arquivos Modificados:
- `.github/workflows/deploy.yml` - Adicionadas variáveis do Cognito no Terraform
- `.github/workflows/publish-lambda.yml` - Adicionadas variáveis de ambiente do Cognito na Lambda
- `src/FiapFastFoodAutenticacao/Core/Models/AdminLoginResponse.cs` - Atualizado para suportar tokens do Cognito
- `src/FiapFastFoodAutenticacao/Core/UseCases/AutenticacaoAdminUseCase.cs` - Implementação com Cognito
- `src/FiapFastFoodAutenticacao/FiapFastFoodAutenticacao.csproj` - Adicionada dependência AWSSDK.CognitoIdentityProvider

### 2. Variáveis Necessárias

#### GitHub Actions - Variables (Repository Settings):
- `AWS_REGION` = "us-east-1"

#### GitHub Actions - Secrets (Repository Settings):
- `TF_VAR_admin_password_permanent` = "12345678" (senha do usuário admin1)
- `COGNITO_USER_POOL_ID` = (será preenchido automaticamente após o deploy do Terraform)
- `COGNITO_CLIENT_ID` = (será preenchido automaticamente após o deploy do Terraform)

### 3. Recursos Criados pelo Terraform

#### Cognito User Pool:
- Nome: `admin-users-fastfood`
- Política de senha: mínimo 8 caracteres, requer números
- Login por username (sem email/SMS)
- MFA desabilitado

#### App Client:
- Nome: `fastfood-admin-client`
- Sem client secret
- Fluxos habilitados: USER_PASSWORD_AUTH, REFRESH_TOKEN_AUTH, USER_SRP_AUTH
- Tokens válidos por 60 minutos (access e id), 30 dias (refresh)

#### Usuário Padrão:
- Username: `admin1`
- Senha: `12345678` (permanente)
- Criado automaticamente sem notificações

### 4. Como Usar

#### Login Admin:
```bash
POST /autenticacaoAdmin
{
  "username": "admin1",
  "password": "12345678"
}
```

#### Resposta:
```json
{
  "success": true,
  "tokenType": "Bearer",
  "accessToken": "eyJraWQiOi...",
  "idToken": "eyJraWQiOi...",
  "expiresIn": 3600,
  "message": "ok"
}
```

### 5. Próximos Passos

1. **Configurar Secrets no GitHub:**
   - Adicionar `TF_VAR_admin_password_permanent` = "12345678"
   - Após o primeiro deploy, adicionar `COGNITO_USER_POOL_ID` e `COGNITO_CLIENT_ID`

2. **Deploy:**
   - Fazer push para branch `main` ou usar `workflow_dispatch`
   - O Terraform criará os recursos do Cognito
   - A Lambda será atualizada com as variáveis de ambiente

3. **Testar:**
   - Usar o endpoint `/autenticacaoAdmin` com admin1/12345678
   - Verificar se retorna token válido do Cognito

### 6. Compatibilidade

- ✅ Mantém compatibilidade com código existente
- ✅ Não quebra funcionalidades atuais
- ✅ Adiciona apenas novas funcionalidades
- ✅ Usa fallback para variáveis de ambiente não configuradas

### 7. Troubleshooting

#### Erro "COGNITO__USERPOOLID not set":
- Verificar se o secret `COGNITO_USER_POOL_ID` está configurado no GitHub
- Verificar se o deploy do Terraform foi executado com sucesso

#### Erro "COGNITO__CLIENTID not set":
- Verificar se o secret `COGNITO_CLIENT_ID` está configurado no GitHub
- Verificar se o deploy do Terraform foi executado com sucesso

#### Erro de autenticação no Cognito:
- Verificar se o usuário admin1 foi criado corretamente
- Verificar se a senha está correta (12345678)
- Verificar se o User Pool e App Client estão ativos
