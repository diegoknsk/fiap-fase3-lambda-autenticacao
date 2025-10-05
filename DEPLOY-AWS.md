# 🚀 Deploy na AWS - FiapFastFoodAutenticacao

## 📋 Pré-requisitos

1. **AWS CLI configurado**
2. **Terraform instalado** (versão 1.0+)
3. **.NET 8 SDK** instalado
4. **PowerShell** (Windows) ou **Bash** (Linux/Mac)

## 🔧 Configuração Inicial

### 1. Configurar AWS CLI
```bash
aws configure
# AWS Access Key ID: [sua access key]
# AWS Secret Access Key: [sua secret key]
# Default region name: us-east-1
# Default output format: json
```

### 2. Verificar credenciais
```bash
aws sts get-caller-identity
```

## 🏗️ Deploy da Infraestrutura

### **Opção 1: Deploy Automático via GitHub Actions (Recomendado)**

#### 1. Configurar Secrets no GitHub
1. Vá para **Settings** → **Secrets and variables** → **Actions**
2. Adicione os secrets listados em `GITHUB-SECRETS.md`:
   - `AWS_ACCESS_KEY_ID`
   - `AWS_SECRET_ACCESS_KEY`
   - `DB_CONNECTION_STRING`
   - `JWT_SECRET`
   - `JWT_ISSUER`
   - `JWT_AUDIENCE`

#### 2. Deploy Automático
```bash
# Deploy automático para produção (apenas main)
git push origin main

# Para desenvolvimento, use dev (apenas build, sem deploy)
git push origin dev
```

#### 3. Deploy Manual (Qualquer Branch)
1. Vá para **Actions** no GitHub
2. Clique em **"Deploy to AWS"**
3. Clique em **"Run workflow"**
4. Escolha a branch desejada
5. Clique em **"Run workflow"**

### **Opção 2: Deploy Manual**

#### 1. Build do Projeto
```bash
# Windows PowerShell
.\build.ps1

# Linux/Mac
./build.sh
```

#### 2. Deploy com Terraform
```bash
# Navegar para o diretório do Terraform
cd infra/terraform

# Inicializar Terraform
terraform init

# Verificar o plano (com secrets)
terraform plan \
  -var="db_connection_string=SEU_CONNECTION_STRING" \
  -var="jwt_secret=SEU_JWT_SECRET" \
  -var="jwt_issuer=FiapFastFood" \
  -var="jwt_audience=FiapFastFood"

# Aplicar a infraestrutura
terraform apply \
  -var="db_connection_string=SEU_CONNECTION_STRING" \
  -var="jwt_secret=SEU_JWT_SECRET" \
  -var="jwt_issuer=FiapFastFood" \
  -var="jwt_audience=FiapFastFood"
```

### 3. Confirmar Deploy
Digite `yes` quando solicitado para confirmar a criação dos recursos.

## 📊 Recursos Criados

Após o deploy, os seguintes recursos serão criados:

- ✅ **Lambda Function** - `FiapFastFoodAutenticacao`
- ✅ **API Gateway** - `FiapFastFoodAutenticacao-api`
- ✅ **IAM Role** - Para execução da Lambda
- ✅ **Secrets Manager** - Para connection string do banco
- ✅ **Políticas IAM** - Para acesso aos secrets

## 🌐 URLs e Endpoints

Após o deploy, você receberá:

### API Gateway URL
```
https://[api-id].execute-api.[region].amazonaws.com/dev
```

### Endpoints Disponíveis
- `POST /autenticacaoAdmin` - Autenticação Admin
- `POST /api/customer/identify` - Identificar Customer
- `POST /api/customer/register` - Registrar Customer
- `POST /api/customer/anonymous` - Registrar Customer Anônimo
- `GET /` - Status da API
- `GET /swagger` - Documentação Swagger

## 🧪 Testando a API

### 1. Teste de Status
```bash
curl https://[api-id].execute-api.[region].amazonaws.com/dev/
```

### 2. Teste Admin
```bash
curl -X POST https://[api-id].execute-api.[region].amazonaws.com/dev/autenticacaoAdmin \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@fiap.com","password":"fiap@2025"}'
```

### 3. Teste Customer Identify
```bash
curl -X POST https://[api-id].execute-api.[region].amazonaws.com/dev/api/customer/identify \
  -H "Content-Type: application/json" \
  -d '{"cpf":"12345678901"}'
```

## 🔄 Atualizações

Para atualizar a Lambda após mudanças no código:

```bash
# 1. Build novamente
.\build.ps1

# 2. Aplicar apenas a Lambda
cd infra/terraform
terraform apply -target=aws_lambda_function.auth
```

## 🗑️ Limpeza (Destruir Recursos)

```bash
cd infra/terraform
terraform destroy
```

## 📝 Logs e Monitoramento

### CloudWatch Logs
- Nome do grupo: `/aws/lambda/FiapFastFoodAutenticacao`
- URL: https://console.aws.amazon.com/cloudwatch/home?region=[region]#logsV2:log-groups

### Métricas
- Invocações
- Duração
- Erros
- Throttles

## 🚨 Troubleshooting

### Erro: "Function not found"
- Verificar se a Lambda foi criada corretamente
- Verificar o nome da função no Terraform

### Erro: "Access Denied"
- Verificar permissões IAM
- Verificar se a role tem as políticas necessárias

### Erro: "Internal Server Error"
- Verificar logs no CloudWatch
- Verificar configuração do API Gateway

### Timeout
- Aumentar timeout da Lambda (atualmente 30s)
- Verificar se o banco está acessível

## 📞 Suporte

Em caso de problemas:
1. Verificar logs no CloudWatch
2. Verificar status dos recursos no console AWS
3. Executar `terraform plan` para verificar mudanças
4. Verificar permissões IAM

---

**✅ Deploy concluído com sucesso!**
