# Admin Lambda Function (sem VPC - acessa Cognito externamente)
resource "aws_lambda_function" "admin" {
  count = var.create_lambda ? 1 : 0
  
  filename         = "${path.module}/../../admin-package.zip"
  function_name    = "FastFoodAutenticacaoAdmin"
  role            = var.lab_role_arn
  handler         = "FiapFastFoodAutenticacao.AdminLambda::FiapFastFoodAutenticacao.AdminLambda.Function::FunctionHandlerAsync"
  source_code_hash = filebase64sha256("${path.module}/../../admin-package.zip")
  runtime         = "dotnet8"
  timeout         = 30
  memory_size     = 512

  # Sem VPC config - acessa Cognito externamente
  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Production"
      COGNITO__USERPOOLID = aws_cognito_user_pool.admins.id
      COGNITO__CLIENTID = aws_cognito_user_pool_client.admins_client.id
    }
  }

  tags = var.tags
}

# Customer Lambda Function (com VPC - acessa RDS)
resource "aws_lambda_function" "customer" {
  count = var.create_lambda ? 1 : 0
  
  filename         = "${path.module}/../../customer-package.zip"
  function_name    = "FastFoodAutenticacaoCustomer"
  role            = var.lab_role_arn
  handler         = "FiapFastFoodAutenticacao.CustomerLambda::FiapFastFoodAutenticacao.CustomerLambda.Function::FunctionHandlerAsync"
  source_code_hash = filebase64sha256("${path.module}/../../customer-package.zip")
  runtime         = "dotnet8"
  timeout         = 30
  memory_size     = 512

  vpc_config {
    subnet_ids         = data.aws_subnets.supported.ids
    security_group_ids = [local.shared_sg_id]
  }

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Production"
      # Configuração JWT específica para Customer
      JwtSettings__Secret = var.jwt_customer_secret
      JwtSettings__Issuer = var.jwt_customer_issuer
      JwtSettings__Audience = var.jwt_customer_audience
      # Connection string do RDS
      RDS_CONNECTION_STRING = var.rds_connection_string
    }
  }

  tags = var.tags
}
