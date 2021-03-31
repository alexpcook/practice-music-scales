output "scales_cluster_endpoint" {
  value = aws_eks_cluster.scales.endpoint
}

output "ecr_repository_username" {
  value = data.aws_ecr_authorization_token.token.user_name
}

output "ecr_repository_password" {
  value = data.aws_ecr_authorization_token.token.password
}

output "ecr_repository_url" {
  value = data.aws_ecr_authorization_token.token.proxy_endpoint
}