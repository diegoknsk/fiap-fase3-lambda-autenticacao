# HTTP API Gateway v2 (apenas se Lambda for criada)
resource "aws_apigatewayv2_api" "fastfood_http" {
  count = var.create_lambda ? 1 : 0
  
  name          = "fastfood-http"
  protocol_type = "HTTP"
  description   = "HTTP API para autenticação FastFood"

  tags = var.tags
}

# Integração com Admin Lambda
resource "aws_apigatewayv2_integration" "admin_lambda" {
  count = var.create_lambda ? 1 : 0
  
  api_id           = aws_apigatewayv2_api.fastfood_http[0].id
  integration_type = "AWS_PROXY"
  integration_uri  = aws_lambda_function.admin[0].invoke_arn
}

# Integração com Customer Lambda
resource "aws_apigatewayv2_integration" "customer_lambda" {
  count = var.create_lambda ? 1 : 0
  
  api_id           = aws_apigatewayv2_api.fastfood_http[0].id
  integration_type = "AWS_PROXY"
  integration_uri  = aws_lambda_function.customer[0].invoke_arn
}

# Rota POST /admin/login -> Admin Lambda
resource "aws_apigatewayv2_route" "admin_login" {
  count = var.create_lambda ? 1 : 0
  
  api_id    = aws_apigatewayv2_api.fastfood_http[0].id
  route_key = "POST /admin/login"
  target    = "integrations/${aws_apigatewayv2_integration.admin_lambda[0].id}"
}

# Rota POST /customer/identify -> Customer Lambda
resource "aws_apigatewayv2_route" "customer_identify" {
  count = var.create_lambda ? 1 : 0
  
  api_id    = aws_apigatewayv2_api.fastfood_http[0].id
  route_key = "POST /customer/identify"
  target    = "integrations/${aws_apigatewayv2_integration.customer_lambda[0].id}"
}

# Rota POST /customer/register -> Customer Lambda
resource "aws_apigatewayv2_route" "customer_register" {
  count = var.create_lambda ? 1 : 0
  
  api_id    = aws_apigatewayv2_api.fastfood_http[0].id
  route_key = "POST /customer/register"
  target    = "integrations/${aws_apigatewayv2_integration.customer_lambda[0].id}"
}

# Rota POST /customer/anonymous -> Customer Lambda
resource "aws_apigatewayv2_route" "customer_anonymous" {
  count = var.create_lambda ? 1 : 0
  
  api_id    = aws_apigatewayv2_api.fastfood_http[0].id
  route_key = "POST /customer/anonymous"
  target    = "integrations/${aws_apigatewayv2_integration.customer_lambda[0].id}"
}

# Stage do HTTP API
resource "aws_apigatewayv2_stage" "fastfood_stage" {
  count = var.create_lambda ? 1 : 0
  
  api_id      = aws_apigatewayv2_api.fastfood_http[0].id
  name        = var.environment
  auto_deploy = true

  tags = var.tags
}

# Permissão para API Gateway invocar Admin Lambda
resource "aws_lambda_permission" "admin_api_gw" {
  count = var.create_lambda ? 1 : 0
  
  statement_id  = "AllowExecutionFromAPIGatewayAdmin"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.admin[0].function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_apigatewayv2_api.fastfood_http[0].execution_arn}/*/*"
}

# Permissão para API Gateway invocar Customer Lambda
resource "aws_lambda_permission" "customer_api_gw" {
  count = var.create_lambda ? 1 : 0
  
  statement_id  = "AllowExecutionFromAPIGatewayCustomer"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.customer[0].function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_apigatewayv2_api.fastfood_http[0].execution_arn}/*/*"
}
