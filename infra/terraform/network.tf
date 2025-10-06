# Pega a VPC default
data "aws_vpc" "default" {
  default = true
}

# Pega todas as subnets da VPC default, exceto a zona 1e (que não é suportada na AWS Academy)
data "aws_subnets" "supported" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
  filter {
    name   = "availability-zone"
    values = ["us-east-1a", "us-east-1b", "us-east-1c", "us-east-1d", "us-east-1f"]
  }
}

# Busca o SG pelo nome fornecido na variável
data "aws_security_groups" "shared_sg" {
  filter {
    name   = "group-name"
    values = [var.shared_sg_name]
  }
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
}

locals {
  shared_sg_id = one(data.aws_security_groups.shared_sg.ids)
}
