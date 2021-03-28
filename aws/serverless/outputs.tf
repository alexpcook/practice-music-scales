output "api_gateway_url" {
  value = "${aws_api_gateway_stage.scales_api.invoke_url}/${var.static_files_index_document}"
}