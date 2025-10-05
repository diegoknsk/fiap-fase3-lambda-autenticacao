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

