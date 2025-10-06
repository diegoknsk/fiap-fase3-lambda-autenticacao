output "admin_lambda_function_name" {
  description = "Nome da função Lambda Admin"
  value       = var.create_lambda ? aws_lambda_function.admin[0].function_name : "FastFoodAutenticacaoAdmin"
}

output "admin_lambda_function_arn" {
  description = "ARN da função Lambda Admin"
  value       = var.create_lambda ? aws_lambda_function.admin[0].arn : "arn:aws:lambda:us-east-1:898384491704:function:FastFoodAutenticacaoAdmin"
}

output "customer_lambda_function_name" {
  description = "Nome da função Lambda Customer"
  value       = var.create_lambda ? aws_lambda_function.customer[0].function_name : "FastFoodAutenticacaoCustomer"
}

output "customer_lambda_function_arn" {
  description = "ARN da função Lambda Customer"
  value       = var.create_lambda ? aws_lambda_function.customer[0].arn : "arn:aws:lambda:us-east-1:898384491704:function:FastFoodAutenticacaoCustomer"
}

output "http_api_gateway_url" {
  description = "URL do HTTP API Gateway"
  value       = var.create_lambda ? "https://${aws_apigatewayv2_api.fastfood_http[0].id}.execute-api.${data.aws_region.current.name}.amazonaws.com/${aws_apigatewayv2_stage.fastfood_stage[0].name}" : "HTTP API Gateway not created (Lambda exists)"
}

output "http_api_gateway_id" {
  description = "ID do HTTP API Gateway"
  value       = var.create_lambda ? aws_apigatewayv2_api.fastfood_http[0].id : "HTTP API Gateway not created (Lambda exists)"
}

# Secrets removidos - usando variáveis de ambiente simples

output "lab_role_arn" {
  description = "ARN da LabRole usada pela Lambda"
  value       = var.lab_role_arn
}

output "lambda_network" {
  description = "Informações de rede da Lambda"
  value = {
    vpc_id       = data.aws_vpc.default.id
    subnet_ids   = data.aws_subnets.supported.ids
    shared_sg_id = local.shared_sg_id
  }
}