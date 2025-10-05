# Data source para região atual
data "aws_region" "current" {}

# Usar LabRole existente da AWS Academy
# Não criamos recursos IAM pois não temos permissão

# Secrets removidos temporariamente - já existem na AWS Academy
# Usar configuração simples via variáveis de ambiente

# Usar LabRole existente - não criamos políticas IAM

# Lambda Function movida para lambda.tf (criação condicional)
# API Gateway movido para api-gateway.tf (criação condicional)