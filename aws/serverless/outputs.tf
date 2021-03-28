output "api_gateway_url" {
  value = "${aws_api_gateway_deployment.scales_api.invoke_url}/${aws_api_gateway_stage.scales_api.id}/${var.static_files_index_document}"
}