variable "project_name" {
  description = "Nome do projeto"
  type        = string
  default     = "FiapFastFoodAutenticacao"
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
    Project     = "FiapFastFoodAutenticacao"
    Environment = "dev"
    ManagedBy   = "terraform"
  }
}

variable "db_connection_string" {
  description = "Connection string do banco de dados"
  type        = string
  sensitive   = true
}

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

