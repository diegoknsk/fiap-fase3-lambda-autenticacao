variable "project_name" {
  description = "Nome do projeto"
  type        = string
  default     = "FastFoodAutenticacao"
}

variable "environment" {
  description = "Ambiente de deploy"
  type        = string
  default     = "dev"
}

variable "tags" {
  description = "Tags padrão para os recursos"
  type        = map(string)
  default = {
    Project     = "FastFoodAutenticacao"
    Environment = "dev"
    ManagedBy   = "terraform"
  }
}

# db_connection_string removido - usando configuração simples

variable "jwt_secret" {
  description = "Secret para geração de JWT tokens"
  type        = string
  sensitive   = true
}

variable "jwt_issuer" {
  description = "Issuer para JWT tokens"
  type        = string
  default     = "FiapFastFood"
}

variable "jwt_audience" {
  description = "Audience para JWT tokens"
  type        = string
  default     = "FiapFastFood"
}

variable "lab_role_arn" {
  description = "ARN da LabRole da AWS Academy"
  type        = string
  default     = "arn:aws:iam::898384491704:role/LabRole"
}

variable "create_lambda" {
  description = "Se deve criar a função Lambda (false se já existir)"
  type        = bool
  default     = true
}

variable "shared_sg_name" {
  description = "Nome do Security Group compartilhado com permissão de acesso ao RDS"
  type        = string
  default     = "eks-shared-to-rds"
}

variable "rds_connection_string" {
  description = "Connection string do RDS MySQL"
  type        = string
  sensitive   = true
  default     = "server=fastfood-rds-mysql.cdiuseg40rpb.us-east-1.rds.amazonaws.com;port=3306;database=fastfooddb;user=admin;password=admin123;SslMode=Preferred"
}

