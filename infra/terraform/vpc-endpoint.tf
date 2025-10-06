# VPC Endpoint para Cognito IdP
resource "aws_vpc_endpoint" "cognito_idp" {
  vpc_id              = data.aws_vpc.default.id
  service_name        = "com.amazonaws.us-east-1.cognito-idp"
  vpc_endpoint_type   = "Interface"
  subnet_ids          = data.aws_subnets.supported.ids
  security_group_ids  = [aws_security_group.cognito_endpoint_sg.id]
  private_dns_enabled = true

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Sid       = "AllowCognitoIdP"
        Effect    = "Allow"
        Principal = "*"
        Action    = "cognito-idp:*"
        Resource  = "*"
      }
    ]
  })

  tags = merge(var.tags, {
    Name = "${var.project_name}-cognito-endpoint"
  })
}

# Security Group para o VPC Endpoint
resource "aws_security_group" "cognito_endpoint_sg" {
  name_prefix = "${var.project_name}-cognito-endpoint-"
  vpc_id      = data.aws_vpc.default.id
  description = "Security group for Cognito VPC Endpoint"

  tags = merge(var.tags, {
    Name = "${var.project_name}-cognito-endpoint-sg"
  })

  lifecycle {
    create_before_destroy = true
  }
}

# Security Group para o Lambda (se não existir)
resource "aws_security_group" "lambda_sg" {
  name_prefix = "${var.project_name}-lambda-"
  vpc_id      = data.aws_vpc.default.id
  description = "Security group for Lambda function"

  tags = merge(var.tags, {
    Name = "${var.project_name}-lambda-sg"
  })

  lifecycle {
    create_before_destroy = true
  }
}

# Regra de ingress no SG do endpoint: permitir TCP 443 a partir do SG do Lambda
resource "aws_security_group_rule" "cognito_ep_ingress_443_from_lambda" {
  type                     = "ingress"
  from_port                = 443
  to_port                  = 443
  protocol                 = "tcp"
  security_group_id        = aws_security_group.cognito_endpoint_sg.id
  source_security_group_id = aws_security_group.lambda_sg.id
  description              = "Allow Lambda SG to reach Cognito VPC Endpoint on 443"
}

# Regra de egress no SG do Lambda: permitir TCP 443
resource "aws_security_group_rule" "lambda_egress_443" {
  type              = "egress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  security_group_id = aws_security_group.lambda_sg.id
  cidr_blocks       = ["0.0.0.0/0"]
  description       = "Allow Lambda egress on 443"
}

# Regra de egress no SG do Lambda para HTTPS (porta 443) específica para o endpoint
resource "aws_security_group_rule" "lambda_egress_443_to_endpoint" {
  type                     = "egress"
  from_port                = 443
  to_port                  = 443
  protocol                 = "tcp"
  security_group_id        = aws_security_group.lambda_sg.id
  source_security_group_id = aws_security_group.cognito_endpoint_sg.id
  description              = "Allow Lambda to reach Cognito VPC Endpoint on 443"
}
