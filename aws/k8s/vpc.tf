resource "aws_default_vpc" "default" {
  tags = {
    Name = join("-", [var.name_prefix, "default", "vpc"])
  }
}

data "aws_subnet_ids" "default" {
  vpc_id = aws_default_vpc.default.id
}