############################
# VARIÁVEIS (apenas se não existirem em variables.tf)
############################
variable "aws_region" {
  type    = string
  default = "us-east-1"
}

variable "cognito_user_pool_name" {
  type    = string
  default = "fastfood-admins"
}

variable "cognito_app_client_name" {
  type    = string
  default = "fastfood-admin-client"
}

variable "admin_username" {
  type    = string
  default = "admin1"
}

# Em lab pode ser default; em CI use secret TF_VAR_admin_password_permanent
variable "admin_password_permanent" {
  type      = string
  sensitive = true
  default   = "12345678"
}

############################
# USER POOL
############################
resource "aws_cognito_user_pool" "admins" {
  name = var.cognito_user_pool_name

  password_policy {
    minimum_length    = 8
    require_lowercase = false
    require_numbers   = true
    require_symbols   = false
    require_uppercase = false
  }

  # Login por username (padrão)
  auto_verified_attributes = []
  mfa_configuration        = "OFF"

  admin_create_user_config {
    allow_admin_create_user_only = true
  }

  tags = {
    Project = "fiap-fastfood"
    Module  = "auth"
  }
}

############################
# APP CLIENT (sem secret) — permite USER_PASSWORD_AUTH
############################
resource "aws_cognito_user_pool_client" "admins_client" {
  name         = var.cognito_app_client_name
  user_pool_id = aws_cognito_user_pool.admins.id

  generate_secret               = false
  prevent_user_existence_errors = "ENABLED"

  explicit_auth_flows = [
    "ALLOW_USER_PASSWORD_AUTH",
    "ALLOW_REFRESH_TOKEN_AUTH",
    "ALLOW_USER_SRP_AUTH"
  ]

  enable_token_revocation = true

  access_token_validity  = 60
  id_token_validity      = 60
  refresh_token_validity = 30

  token_validity_units {
    access_token  = "minutes"
    id_token      = "minutes"
    refresh_token = "days"
  }
}

############################
# USUÁRIO PADRÃO admin1 (sem e-mail/sms)
############################
resource "aws_cognito_user" "admin_user" {
  user_pool_id   = aws_cognito_user_pool.admins.id
  username       = var.admin_username
  message_action = "SUPPRESS" # não enviar notificação
}

############################
# SENHA PERMANENTE via AWS CLI (runner precisa ter aws cli)
############################
resource "null_resource" "set_admin_permanent_password" {
  triggers = {
    user_pool_id = aws_cognito_user_pool.admins.id
    username     = aws_cognito_user.admin_user.username
    password     = var.admin_password_permanent
    region       = var.aws_region
  }

  provisioner "local-exec" {
    command = <<EOT
aws cognito-idp admin-set-user-password \
  --user-pool-id ${aws_cognito_user_pool.admins.id} \
  --username ${aws_cognito_user.admin_user.username} \
  --password '${var.admin_password_permanent}' \
  --permanent \
  --region ${var.aws_region}
EOT
    environment = {
      AWS_DEFAULT_REGION = var.aws_region
    }
  }

  depends_on = [aws_cognito_user.admin_user]
}

############################
# OUTPUTS (adicionar aqui; se já existir outputs.tf, pode manter nestes também)
############################
output "cognito_user_pool_id" {
  value = aws_cognito_user_pool.admins.id
}

output "cognito_app_client_id" {
  value = aws_cognito_user_pool_client.admins_client.id
}

output "cognito_admin_username" {
  value = aws_cognito_user.admin_user.username
}

output "cognito_jwks_url" {
  value = "https://cognito-idp.${var.aws_region}.amazonaws.com/${aws_cognito_user_pool.admins.id}/.well-known/jwks.json"
}
