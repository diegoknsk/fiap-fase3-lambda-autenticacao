output "lambda_admin_function_name" {
  description = "Nome da função Lambda Admin"
  value       = aws_lambda_function.admin.function_name
}

output "lambda_admin_arn" {
  description = "ARN da função Lambda Admin"
  value       = aws_lambda_function.admin.arn
}

output "lambda_totem_function_name" {
  description = "Nome da função Lambda Totem"
  value       = aws_lambda_function.totem.function_name
}

output "lambda_totem_arn" {
  description = "ARN da função Lambda Totem"
  value       = aws_lambda_function.totem.arn
}

output "secret_arn" {
  description = "ARN do secret no AWS Secrets Manager"
  value       = aws_secretsmanager_secret.connection_string.arn
}

output "secret_name" {
  description = "Nome do secret no AWS Secrets Manager"
  value       = aws_secretsmanager_secret.connection_string.name
}

output "iam_role_arn" {
  description = "ARN da IAM Role do Lambda"
  value       = aws_iam_role.lambda_exec.arn
}

