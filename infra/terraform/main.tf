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
    connection_string = "server=fastfood-rds-mysql.cdiuseg40rpb.us-east-1.rds.amazonaws.com;port=3306;database=fastfooddb;user=admin;password=admin123;SslMode=Preferred"
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
        Resource = aws_secretsmanager_secret.connection_string.arn
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
    }
  }

  tags = var.tags
}

