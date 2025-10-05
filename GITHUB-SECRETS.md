# 🔐 GitHub Secrets Configuration

## 📋 Secrets Necessários no GitHub

Para configurar o deploy automático via GitHub Actions, você precisa adicionar os seguintes secrets no seu repositório GitHub:

### 🔧 Como Adicionar Secrets

1. Vá para o seu repositório no GitHub
2. Clique em **Settings** (Configurações)
3. No menu lateral, clique em **Secrets and variables** → **Actions**
4. Clique em **New repository secret**
5. Adicione cada secret abaixo:

### 📝 Lista de Secrets

#### **AWS Credentials (AWS Academy)**
```
AWS_ACCESS_KEY_ID
```
- **Descrição:** Access Key ID temporária da AWS Academy
- **Exemplo:** `ASIAIOSFODNN7EXAMPLE`

```
AWS_SECRET_ACCESS_KEY
```
- **Descrição:** Secret Access Key temporária da AWS Academy
- **Exemplo:** `wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY`

```
AWS_SESSION_TOKEN
```
- **Descrição:** Session Token temporário da AWS Academy
- **Exemplo:** `IQoJb3JpZ2luX2VjE...` (token longo)

#### **Database Configuration**
```
DB_CONNECTION_STRING
```
- **Descrição:** Connection string do banco de dados
- **Exemplo:** `server=fastfood-rds-mysql.cdiuseg40rpb.us-east-1.rds.amazonaws.com;port=3306;database=fastfooddb;user=admin;password=admin123;SslMode=Preferred`

#### **JWT Configuration**
```
JWT_SECRET
```
- **Descrição:** Secret para geração de JWT tokens
- **Exemplo:** `FiapFastFoodSuperSecretKeyForJWTTokenGeneration2025!`

```
JWT_ISSUER
```
- **Descrição:** Issuer para JWT tokens
- **Exemplo:** `FiapFastFood`

```
JWT_AUDIENCE
```
- **Descrição:** Audience para JWT tokens
- **Exemplo:** `FiapFastFood`

## 🎓 AWS Academy - Configuração Especial

### **⚠️ Importante para AWS Academy:**
- As credenciais da AWS Academy são **temporárias** (expira em algumas horas)
- Você precisará **renovar as credenciais** periodicamente
- O **Session Token** é obrigatório para AWS Academy

### **🔄 Renovação de Credenciais:**
1. Acesse seu **AWS Academy Learner Lab**
2. Clique em **"AWS Details"** → **"Show"**
3. Copie as novas credenciais
4. Atualize os secrets no GitHub:
   - `AWS_ACCESS_KEY_ID`
   - `AWS_SECRET_ACCESS_KEY`
   - `AWS_SESSION_TOKEN`

### **⏰ Dica:**
- Configure um lembrete para renovar as credenciais
- As credenciais expiram automaticamente
- Sem credenciais válidas, o deploy falhará

### **🛠️ Scripts de Atualização:**
Para facilitar a renovação das credenciais, use os scripts:

#### **Windows PowerShell:**
```powershell
.\scripts\update-aws-credentials.ps1 -AccessKeyId "ASIA..." -SecretAccessKey "wJalr..." -SessionToken "IQoJb3JpZ2luX2VjE..."
```

#### **Linux/Mac:**
```bash
./scripts/update-aws-credentials.sh ASIA... wJalr... IQoJb3JpZ2luX2VjE...
```

**Pré-requisito:** Instalar GitHub CLI (`gh`) e fazer login

## 🚀 Como Funciona o Deploy

### **Trigger Automático**
- **Push para `main`** → Deploy para produção
- **Push para `dev`** → Deploy para desenvolvimento
- **Pull Request** → Preview do deploy (comentário na PR)

### **Processo de Deploy**
1. **Build** - Compila o projeto .NET
2. **Package** - Cria o ZIP para Lambda
3. **Terraform Init** - Inicializa o Terraform
4. **Terraform Plan** - Verifica as mudanças
5. **Terraform Apply** - Aplica a infraestrutura
6. **Comment PR** - Comenta na PR com a URL da API

## 🔒 Segurança

### **Secrets Sensíveis**
- Todos os secrets são marcados como `sensitive = true` no Terraform
- Os valores não aparecem nos logs do GitHub Actions
- Os secrets são armazenados de forma segura no AWS Secrets Manager

### **IAM Permissions**
- A Lambda tem permissões mínimas necessárias
- Apenas leitura dos secrets específicos
- Sem acesso a outros recursos AWS

## 📊 Monitoramento

### **Logs do Deploy**
- Logs disponíveis na aba **Actions** do GitHub
- Logs da Lambda disponíveis no CloudWatch

### **Status do Deploy**
- ✅ Verde: Deploy bem-sucedido
- ❌ Vermelho: Erro no deploy
- 🟡 Amarelo: Deploy em andamento

## 🛠️ Troubleshooting

### **Erro: "Secret not found"**
- Verificar se todos os secrets foram adicionados
- Verificar se os nomes estão corretos (case-sensitive)

### **Erro: "AWS credentials invalid"**
- Verificar se as credenciais AWS estão corretas
- Verificar se a conta tem as permissões necessárias

### **Erro: "Terraform plan failed"**
- Verificar se os valores dos secrets estão no formato correto
- Verificar se a região AWS está configurada corretamente

## 📞 Suporte

Em caso de problemas:
1. Verificar logs na aba **Actions**
2. Verificar se todos os secrets estão configurados
3. Verificar permissões AWS
4. Verificar formato dos secrets

---

**✅ Configuração concluída!**
