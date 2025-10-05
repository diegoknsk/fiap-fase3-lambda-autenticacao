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
- **POST** `/api/admin/login` - Login Admin (simula Lambda)
- **POST** `/api/admin/test-valid` - Teste com credenciais válidas
- **POST** `/api/admin/test-invalid` - Teste com credenciais inválidas

#### Totem Handler  
- **POST** `/api/totem/login` - Login Totem (simula Lambda)
- **POST** `/api/totem/test-valid` - Teste com credenciais válidas
- **POST** `/api/totem/test-invalid` - Teste com credenciais inválidas
- **POST** `/api/totem/test-cpf-not-found` - Teste CPF não encontrado

## 🔍 Debug e Logs

A API simula **exatamente** como o AWS Lambda chama os handlers:

1. **Serialização/Deserialização JSON** igual ao Lambda
2. **Contexto Lambda** mockado com `ILambdaContext`
3. **Logs detalhados** mostrando cada passo
4. **Environment variables** configuradas como em produção

## 📝 Exemplo de Request

### Admin Login
```json
POST /api/admin/login
{
  "username": "admin",
  "password": "fiap@2025"
}
```

### Totem Login
```json
POST /api/totem/login
{
  "cpf": "12345678901",
  "senha": "1234"
}
```

## 🎯 Credenciais de Teste

### Admin (Mock Cognito)
- **Username:** `admin`
- **Password:** `fiap@2025`

### Totem (Mock MySQL)
- **CPF:** `12345678901`
- **Senha:** `1234`

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
