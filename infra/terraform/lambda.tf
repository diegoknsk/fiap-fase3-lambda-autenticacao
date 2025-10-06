# Lambda Function - apenas se não existir
resource "aws_lambda_function" "auth" {
  count = var.create_lambda ? 1 : 0
  
  filename         = "${path.module}/../../package.zip"
  function_name    = var.project_name
  role            = var.lab_role_arn
  handler         = "FiapFastFoodAutenticacao::FiapFastFoodAutenticacao.Handlers.Dispatcher::FunctionHandlerAsync"
  source_code_hash = filebase64sha256("${path.module}/../../package.zip")
  runtime         = "dotnet8"
  timeout         = 30
  memory_size     = 512

  vpc_config {
    subnet_ids         = data.aws_subnets.supported.ids
    security_group_ids = [aws_security_group.lambda_sg.id, local.shared_sg_id]
  }

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Production"
      # Configuração JWT via variáveis de ambiente (simples)
      JwtSettings__Secret = var.jwt_secret
      JwtSettings__Issuer = var.jwt_issuer
      JwtSettings__Audience = var.jwt_audience
    }
  }

  tags = var.tags
}
