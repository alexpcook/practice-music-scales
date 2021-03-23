output "api_gateway_url" {
  value = aws_api_gateway_deployment.scales_api.invoke_url
}