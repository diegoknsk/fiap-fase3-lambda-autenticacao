# 🧪 Teste de Conexão MySQL - Customer Lambda

## 📋 O que foi implementado

Adicionei um **teste de conexão MySQL** na Customer Lambda que será executado automaticamente sempre que a rota `/customer/identify` for chamada.

## 🔧 Como funciona

### 1. String de Conexão de Teste
```csharp
"server=fastfood-rds-mysql.cdiuseg40rpb.us-east-1.rds.amazonaws.com;port=3306;database=fastfooddb;user=admin;password=admin123;SslMode=Preferred"
```

### 2. Lógica do Teste
- **Primeiro**: Tenta usar a variável de ambiente `RDS_CONNECTION_STRING`
- **Fallback**: Se não existir, usa a string de conexão de teste fornecida
- **Teste**: Abre conexão + executa query `SELECT 1 as test`
- **Logs**: Registra resultado detalhado nos logs da Lambda

### 3. Logs Gerados
```
⚠️ RDS_CONNECTION_STRING não configurada, usando string de teste
Connection String: server=fastfood-rds-mysql.cdiuseg40rpb.us-east-1.rds.amazonaws.com;port=3306;database=fastfooddb;user=admin;password=***
Testando conexão com o banco de dados...
✅ Conexão com banco estabelecida com sucesso!
✅ Query de teste executada: 1
Teste de conexão com banco: SUCESSO - Conectado ao banco e query executada
```

## 🚀 Como testar

### 1. Build e Deploy
```bash
# Build das Lambdas
.\build.ps1  # Windows
./build.sh   # Linux/macOS

# Deploy via Terraform
cd infra/terraform
terraform init
terraform apply
```

### 2. Testar a Rota
```bash
# Fazer uma chamada para /customer/identify
curl -X POST https://[API_GATEWAY_URL]/customer/identify \
  -H "Content-Type: application/json" \
  -d '{"cpf": "12345678901"}'
```

### 3. Verificar Logs
- Acesse AWS CloudWatch Logs
- Procure pelo log group da Lambda `FastFoodAutenticacaoCustomer`
- Verifique se aparecem as mensagens de teste de conexão

## 🔍 O que o teste valida

1. **✅ Conectividade de Rede**: Lambda consegue alcançar o RDS
2. **✅ Credenciais**: Usuário/senha estão corretos
3. **✅ Database**: Database `fastfooddb` existe
4. **✅ Permissões**: Usuário tem permissão para executar queries
5. **✅ SSL**: Conexão SSL está funcionando

## 📊 Resultados Esperados

### ✅ Sucesso
```
Teste de conexão com banco: SUCESSO - Conectado ao banco e query executada
```

### ❌ Falha
```
Teste de conexão com banco: FALHA - Erro: [detalhes do erro]
```

## 🛠️ Troubleshooting

### Erro de Conectividade
- Verificar se a Lambda está na VPC correta
- Verificar Security Groups
- Verificar se o RDS está acessível

### Erro de Credenciais
- Verificar usuário/senha
- Verificar se o usuário tem permissões

### Erro de Database
- Verificar se o database `fastfooddb` existe
- Verificar se o usuário tem acesso ao database

## 📝 Próximos Passos

Após confirmar que a conexão está funcionando:

1. **Configurar variável de ambiente** no Terraform
2. **Remover string hardcoded** do código
3. **Implementar operações reais** no banco
4. **Adicionar mais rotas** (`/customer/register`, `/customer/anonymous`)

## 🔒 Segurança

- A senha é **mascarada** nos logs
- A string de teste é apenas para **validação inicial**
- Em produção, usar **AWS Secrets Manager** ou variáveis de ambiente
