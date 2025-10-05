output "lambda_function_name" {
  description = "Nome da função Lambda"
  value       = var.create_lambda ? aws_lambda_function.auth[0].function_name : "FastFoodAutenticacao"
}

output "lambda_function_arn" {
  description = "ARN da função Lambda"
  value       = var.create_lambda ? aws_lambda_function.auth[0].arn : "arn:aws:lambda:us-east-1:898384491704:function:FastFoodAutenticacao"
}

output "api_gateway_url" {
  description = "URL do API Gateway"
  value       = "https://${aws_api_gateway_rest_api.auth_api.id}.execute-api.${data.aws_region.current.name}.amazonaws.com/${aws_api_gateway_stage.auth_stage.stage_name}"
}

output "api_gateway_id" {
  description = "ID do API Gateway"
  value       = aws_api_gateway_rest_api.auth_api.id
}

# Secrets removidos - usando variáveis de ambiente simples

output "lab_role_arn" {
  description = "ARN da LabRole usada pela Lambda"
  value       = var.lab_role_arn
}