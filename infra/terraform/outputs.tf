output "lambda_function_name" {
  description = "Nome da função Lambda"
  value       = aws_lambda_function.auth.function_name
}

output "lambda_function_arn" {
  description = "ARN da função Lambda"
  value       = aws_lambda_function.auth.arn
}

output "api_gateway_url" {
  description = "URL do API Gateway"
  value       = "https://${aws_api_gateway_rest_api.auth_api.id}.execute-api.${data.aws_region.current.name}.amazonaws.com/${var.environment}"
}

output "api_gateway_id" {
  description = "ID do API Gateway"
  value       = aws_api_gateway_rest_api.auth_api.id
}

output "db_secret_arn" {
  description = "ARN do secret de conexão do banco"
  value       = aws_secretsmanager_secret.connection_string.arn
}

output "jwt_secret_arn" {
  description = "ARN do secret de configurações JWT"
  value       = aws_secretsmanager_secret.jwt_settings.arn
}

output "db_secret_name" {
  description = "Nome do secret de conexão do banco"
  value       = aws_secretsmanager_secret.connection_string.name
}

output "jwt_secret_name" {
  description = "Nome do secret de configurações JWT"
  value       = aws_secretsmanager_secret.jwt_settings.name
}

output "iam_role_arn" {
  description = "ARN da IAM Role do Lambda"
  value       = aws_iam_role.lambda_exec.arn
}