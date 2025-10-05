# 🚀 Instruções para Debug Local

## ✅ **Problema Resolvido!**

O projeto **FiapFastFoodAutenticacao.DebugApi** agora está visível no Visual Studio e funcionando perfeitamente **COM SWAGGER**!

## 🎯 **Como usar:**

### **1. No Visual Studio:**
- Abra a solution `FiapFastFoodAutenticacao.sln`
- Você verá 3 projetos:
  - `FiapFastFoodAutenticacao` (Core - fonte única de lógica)
  - `FiapFastFoodAutenticacao.DebugApi` (Host de debug **COM SWAGGER**)
  - `FiapFastFoodAutenticacao.Tests` (Testes)

### **2. Executar DebugApi com Swagger:**
- Clique com botão direito no projeto `FiapFastFoodAutenticacao.DebugApi`
- Selecione "Set as Startup Project"
- Pressione **F5** ou clique em "Start Debugging"

### **3. Testar endpoints no Swagger:**
- Acesse: **http://localhost:5000** (Swagger UI)
- Teste os endpoints diretamente na interface:
  - **POST** `/autenticacaoAdmin`
    ```json
    {
      "email": "admin@fiap.com",
      "password": "fiap@2025"
    }
    ```
  - **POST** `/autenticacaoTotem`
    ```json
    {
      "cpf": "12345678901"
    }
    ```

## 🏗️ **Arquitetura Implementada:**

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
```

## 🎯 **Princípios Atendidos:**

✅ **Fonte única de lógica** - Toda regra de negócio no projeto Core  
✅ **DebugApi é apenas host** - Nenhuma regra de negócio no DebugApi  
✅ **Deploy apenas Lambda** - DebugApi não vai para produção  
✅ **Secrets via AWS Secrets Manager** - Nada sensível no repo  
✅ **Clean Architecture simples** - Sem DI pesado entre camadas  

## 🚀 **Deploy para Produção:**

```bash
git add .
git commit -m "feat: implementar clean architecture"
git push origin main
# GitHub Actions fará o deploy automático para o Lambda
```

## 📋 **Credenciais de Teste:**

- **Admin:** `admin@fiap.com` / `fiap@2025`
- **Totem:** `12345678901`

---

**🎉 Agora você pode debugar localmente como uma API normal, mas com a mesma lógica que vai para produção no Lambda!**
