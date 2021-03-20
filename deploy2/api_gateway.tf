resource "aws_api_gateway_rest_api" "scales_api" {
  name = join("-", [var.name_prefix, "gateway"])
}

resource "aws_api_gateway_resource" "scales_api" {
  parent_id   = aws_api_gateway_rest_api.scales_api.root_resource_id
  path_part   = "{bucketkey}"
  rest_api_id = aws_api_gateway_rest_api.scales_api.id
}

resource "aws_api_gateway_method" "scales_api" {
  http_method   = "GET"
  authorization = "NONE"
  resource_id   = aws_api_gateway_resource.scales_api.id
  rest_api_id   = aws_api_gateway_rest_api.scales_api.id
  request_parameters = {
    "method.request.path.bucketkey" = false
  }
}