# Data source para região atual
data "aws_region" "current" {}

# IAM Role para Lambda
resource "aws_iam_role" "lambda_exec" {
  name = "${var.project_name}-lambda-exec-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      }
    ]
  })

  tags = var.tags
}

# Anexar política básica de execução do Lambda
resource "aws_iam_role_policy_attachment" "basic_exec" {
  role       = aws_iam_role.lambda_exec.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

# Secret no AWS Secrets Manager
resource "aws_secretsmanager_secret" "connection_string" {
  name        = "fastfood/db/connection-string"
  description = "Connection string para o banco de dados MySQL do FastFood"

  tags = var.tags
}

resource "aws_secretsmanager_secret_version" "connection_string_value" {
  secret_id = aws_secretsmanager_secret.connection_string.id
  secret_string = jsonencode({
    connection_string = var.db_connection_string
  })
}

# Secret para configurações JWT
resource "aws_secretsmanager_secret" "jwt_settings" {
  name        = "fastfood/jwt/settings"
  description = "Configurações JWT para autenticação FastFood"

  tags = var.tags
}

resource "aws_secretsmanager_secret_version" "jwt_settings_value" {
  secret_id = aws_secretsmanager_secret.jwt_settings.id
  secret_string = jsonencode({
    secret   = var.jwt_secret
    issuer   = var.jwt_issuer
    audience = var.jwt_audience
  })
}

# Política para ler secrets
resource "aws_iam_policy" "secrets_read_policy" {
  name        = "${var.project_name}-secrets-read-policy"
  description = "Política para ler secrets do AWS Secrets Manager"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "secretsmanager:GetSecretValue"
        ]
        Resource = [
          aws_secretsmanager_secret.connection_string.arn,
          aws_secretsmanager_secret.jwt_settings.arn
        ]
      }
    ]
  })

  tags = var.tags
}

# Anexar política de secrets à role do Lambda
resource "aws_iam_role_policy_attachment" "secrets_read_attach" {
  role       = aws_iam_role.lambda_exec.name
  policy_arn = aws_iam_policy.secrets_read_policy.arn
}

# Lambda Function única com ASP.NET Core
resource "aws_lambda_function" "auth" {
  filename         = "${path.module}/../../package.zip"
  function_name    = var.project_name
  role            = aws_iam_role.lambda_exec.arn
  handler         = "FiapFastFoodAutenticacao::LambdaEntryPoint::FunctionHandlerAsync"
  source_code_hash = filebase64sha256("${path.module}/../../package.zip")
  runtime         = "dotnet8"
  timeout         = 30
  memory_size     = 512

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Production"
      SECRET_CONNECTION_STRING_ARN = aws_secretsmanager_secret.connection_string.arn
      SECRET_JWT_SETTINGS_ARN = aws_secretsmanager_secret.jwt_settings.arn
    }
  }

  tags = var.tags
}

# API Gateway
resource "aws_api_gateway_rest_api" "auth_api" {
  name        = "${var.project_name}-api"
  description = "API Gateway para autenticação FastFood"

  endpoint_configuration {
    types = ["REGIONAL"]
  }

  tags = var.tags
}

# Recurso raiz
resource "aws_api_gateway_resource" "proxy" {
  rest_api_id = aws_api_gateway_rest_api.auth_api.id
  parent_id   = aws_api_gateway_rest_api.auth_api.root_resource_id
  path_part   = "{proxy+}"
}

# Método ANY para o proxy
resource "aws_api_gateway_method" "proxy" {
  rest_api_id   = aws_api_gateway_rest_api.auth_api.id
  resource_id   = aws_api_gateway_resource.proxy.id
  http_method   = "ANY"
  authorization = "NONE"
}

# Método ANY para a raiz
resource "aws_api_gateway_method" "proxy_root" {
  rest_api_id   = aws_api_gateway_rest_api.auth_api.id
  resource_id   = aws_api_gateway_rest_api.auth_api.root_resource_id
  http_method   = "ANY"
  authorization = "NONE"
}

# Integração Lambda para proxy
resource "aws_api_gateway_integration" "lambda" {
  rest_api_id = aws_api_gateway_rest_api.auth_api.id
  resource_id = aws_api_gateway_method.proxy.resource_id
  http_method = aws_api_gateway_method.proxy.http_method

  integration_http_method = "POST"
  type                   = "AWS_PROXY"
  uri                    = aws_lambda_function.auth.invoke_arn
}

# Integração Lambda para raiz
resource "aws_api_gateway_integration" "lambda_root" {
  rest_api_id = aws_api_gateway_rest_api.auth_api.id
  resource_id = aws_api_gateway_method.proxy_root.resource_id
  http_method = aws_api_gateway_method.proxy_root.http_method

  integration_http_method = "POST"
  type                   = "AWS_PROXY"
  uri                    = aws_lambda_function.auth.invoke_arn
}

# Permissão para API Gateway invocar Lambda
resource "aws_lambda_permission" "api_gw" {
  statement_id  = "AllowExecutionFromAPIGateway"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.auth.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.auth_api.execution_arn}/*/*"
}

# Deploy do API Gateway
resource "aws_api_gateway_deployment" "auth_deployment" {
  depends_on = [
    aws_api_gateway_integration.lambda,
    aws_api_gateway_integration.lambda_root,
  ]

  rest_api_id = aws_api_gateway_rest_api.auth_api.id
  stage_name  = var.environment

  lifecycle {
    create_before_destroy = true
  }
}

