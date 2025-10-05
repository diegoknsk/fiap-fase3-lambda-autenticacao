# Data source para região atual
data "aws_region" "current" {}

# Usar LabRole existente da AWS Academy
# Não criamos recursos IAM pois não temos permissão

# Secrets removidos temporariamente - já existem na AWS Academy
# Usar configuração simples via variáveis de ambiente

# Usar LabRole existente - não criamos políticas IAM

# Lambda Function movida para lambda.tf (criação condicional)

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

  lifecycle {
    create_before_destroy = true
  }
}

# Stage do API Gateway (substitui stage_name deprecated)
resource "aws_api_gateway_stage" "auth_stage" {
  deployment_id = aws_api_gateway_deployment.auth_deployment.id
  rest_api_id   = aws_api_gateway_rest_api.auth_api.id
  stage_name    = var.environment
}

