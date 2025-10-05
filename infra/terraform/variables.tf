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

