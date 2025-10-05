# API Gateway (apenas se Lambda for criada)
resource "aws_api_gateway_rest_api" "auth_api" {
  count = var.create_lambda ? 1 : 0
  
  name        = "${var.project_name}-api"
  description = "API Gateway para autenticação FastFood"

  endpoint_configuration {
    types = ["REGIONAL"]
  }

  tags = var.tags
}

# Recurso raiz
resource "aws_api_gateway_resource" "proxy" {
  count = var.create_lambda ? 1 : 0
  
  rest_api_id = aws_api_gateway_rest_api.auth_api[0].id
  parent_id   = aws_api_gateway_rest_api.auth_api[0].root_resource_id
  path_part   = "{proxy+}"
}

# Método para proxy
resource "aws_api_gateway_method" "proxy" {
  count = var.create_lambda ? 1 : 0
  
  rest_api_id   = aws_api_gateway_rest_api.auth_api[0].id
  resource_id   = aws_api_gateway_resource.proxy[0].id
  http_method   = "ANY"
  authorization = "NONE"
}

# Método para raiz
resource "aws_api_gateway_method" "proxy_root" {
  count = var.create_lambda ? 1 : 0
  
  rest_api_id   = aws_api_gateway_rest_api.auth_api[0].id
  resource_id   = aws_api_gateway_rest_api.auth_api[0].root_resource_id
  http_method   = "ANY"
  authorization = "NONE"
}

# Integração Lambda para proxy
resource "aws_api_gateway_integration" "lambda" {
  count = var.create_lambda ? 1 : 0
  
  rest_api_id = aws_api_gateway_rest_api.auth_api[0].id
  resource_id = aws_api_gateway_method.proxy[0].resource_id
  http_method = aws_api_gateway_method.proxy[0].http_method

  integration_http_method = "POST"
  type                   = "AWS_PROXY"
  uri                    = aws_lambda_function.auth[0].invoke_arn
}

# Integração Lambda para raiz
resource "aws_api_gateway_integration" "lambda_root" {
  count = var.create_lambda ? 1 : 0
  
  rest_api_id = aws_api_gateway_rest_api.auth_api[0].id
  resource_id = aws_api_gateway_method.proxy_root[0].resource_id
  http_method = aws_api_gateway_method.proxy_root[0].http_method

  integration_http_method = "POST"
  type                   = "AWS_PROXY"
  uri                    = aws_lambda_function.auth[0].invoke_arn
}

# Permissão para API Gateway invocar Lambda (apenas se Lambda for criada)
resource "aws_lambda_permission" "api_gw" {
  count = var.create_lambda ? 1 : 0
  
  statement_id  = "AllowExecutionFromAPIGateway"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.auth[0].function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.auth_api[0].execution_arn}/*/*"
}

# Deploy do API Gateway
resource "aws_api_gateway_deployment" "auth_deployment" {
  count = var.create_lambda ? 1 : 0
  
  depends_on = [
    aws_api_gateway_integration.lambda[0],
    aws_api_gateway_integration.lambda_root[0],
  ]

  rest_api_id = aws_api_gateway_rest_api.auth_api[0].id

  lifecycle {
    create_before_destroy = true
    ignore_changes = [triggers]
  }

  triggers = {
    redeployment = sha1(jsonencode([
      aws_api_gateway_integration.lambda,
      aws_api_gateway_integration.lambda_root,
    ]))
  }
}

# Stage do API Gateway (substitui stage_name deprecated)
resource "aws_api_gateway_stage" "auth_stage" {
  count = var.create_lambda ? 1 : 0
  
  deployment_id = aws_api_gateway_deployment.auth_deployment[0].id
  rest_api_id   = aws_api_gateway_rest_api.auth_api[0].id
  stage_name    = var.environment

  tags = var.tags
}
