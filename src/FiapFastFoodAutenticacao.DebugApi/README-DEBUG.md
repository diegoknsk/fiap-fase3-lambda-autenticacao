# 🚀 API de Debug - FiapFastFood Lambda

Esta API permite testar e debugar os handlers do Lambda localmente, exatamente como se fossem chamados em produção.

## 🎯 Como Usar

### 1. Executar a API
```bash
cd src/FiapFastFoodAutenticacao.DebugApi
dotnet run
```

### 2. Acessar o Swagger
Abra no navegador: **http://localhost:5000**

### 3. Endpoints Disponíveis

#### Admin Handler
- **POST** `/autenticacaoAdmin` - Login Admin (simula Lambda)

#### Customer Endpoints (Totem)
**IMPORTANTE:** O antigo endpoint de autenticação do totem foi removido. Agora o totem usa os 3 endpoints de Customer:

- **POST** `/api/customer/identify` - Identificar customer por CPF
- **POST** `/api/customer/register` - Registrar novo customer  
- **POST** `/api/customer/anonymous` - Registrar customer anônimo

Todos os endpoints retornam `CustomerTokenResponseModel` com JWT token válido por 3 horas.

#### Endpoints de Teste
- **POST** `/test/admin/valid` - Teste Admin com credenciais válidas
- **POST** `/test/admin/invalid` - Teste Admin com credenciais inválidas
- **POST** `/test/customer/identify-valid` - Teste Identify com CPF válido
- **POST** `/test/customer/identify-invalid` - Teste Identify com CPF inválido
- **POST** `/test/customer/register-valid` - Teste Register com dados válidos
- **POST** `/test/customer/register-existing` - Teste Register com CPF existente
- **POST** `/test/customer/anonymous` - Teste Register Anonymous

## 🔍 Debug e Logs

A API simula **exatamente** como o AWS Lambda chama os handlers:

1. **Serialização/Deserialização JSON** igual ao Lambda
2. **Contexto Lambda** mockado com `ILambdaContext`
3. **Logs detalhados** mostrando cada passo
4. **Environment variables** configuradas como em produção

## 📝 Exemplo de Request

### Admin Login
```json
POST /autenticacaoAdmin
{
  "email": "admin@fiap.com",
  "password": "fiap@2025"
}
```

### Customer Identify
```json
POST /api/customer/identify
{
  "cpf": "12345678901"
}
```

### Customer Register
```json
POST /api/customer/register
{
  "name": "João Silva",
  "email": "joao@email.com",
  "cpf": "12345678901"
}
```

### Customer Anonymous
```json
POST /api/customer/anonymous
```

### Testes Automáticos
```json
POST /test/admin/valid
POST /test/admin/invalid
POST /test/customer/identify-valid
POST /test/customer/identify-invalid
POST /test/customer/register-valid
POST /test/customer/register-existing
POST /test/customer/anonymous
```

## 🎯 Credenciais de Teste

### Admin (Mock Cognito)
- **Email:** `admin@fiap.com`
- **Password:** `fiap@2025`

### Customer (Mock MySQL)
- **CPF:** `12345678901` (para identify)
- **CPF:** Qualquer CPF válido (para register)
- **Anonymous:** Sem parâmetros necessários

## 🔧 Diferença das Arquiteturas

### Seu Projeto (Lambda Handlers)
- ✅ **Serverless** - paga apenas pelo uso
- ✅ **Escalabilidade automática**
- ✅ **Cold start rápido**
- ❌ **Debug mais complexo** (por isso criamos esta API!)

### Projeto Pricefy (API Tradicional)
- ✅ **Debug fácil** - roda como aplicação normal
- ✅ **Swagger nativo**
- ✅ **Middleware completo**
- ❌ **Sempre rodando** (custo fixo)

## 🚀 Deploy para Produção

Quando estiver satisfeito com os testes:

1. **Commit** as mudanças
2. **Push** para o repositório
3. **GitHub Actions** fará o deploy automático para o Lambda

A API de debug **NÃO** vai para produção - é apenas para desenvolvimento local!
