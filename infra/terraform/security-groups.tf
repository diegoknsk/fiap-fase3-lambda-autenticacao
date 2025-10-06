# Regra de egress no SG do Lambda para HTTPS (porta 443) - acesso ao Cognito
resource "aws_security_group_rule" "lambda_egress_https" {
  type              = "egress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  security_group_id = local.shared_sg_id
  cidr_blocks       = ["0.0.0.0/0"]
  description       = "Allow Lambda egress HTTPS for Cognito access"
}

# Regra de egress no SG do Lambda para HTTP (porta 80) - caso necessário
resource "aws_security_group_rule" "lambda_egress_http" {
  type              = "egress"
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  security_group_id = local.shared_sg_id
  cidr_blocks       = ["0.0.0.0/0"]
  description       = "Allow Lambda egress HTTP"
}
