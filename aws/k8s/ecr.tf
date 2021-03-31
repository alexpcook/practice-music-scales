resource "aws_ecr_repository" "scales" {
  count = length(var.ecr_repository_names)

  name                 = var.ecr_repository_names[count.index]
  image_tag_mutability = "MUTABLE"

  encryption_configuration {
    encryption_type = "AES256"
  }

  image_scanning_configuration {
    scan_on_push = true
  }
}

data "aws_ecr_authorization_token" "token" {
}