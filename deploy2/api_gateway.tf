resource "aws_api_gateway_rest_api" "scales_api" {
  name = join("-", [var.name_prefix, "gateway"])
}

# S3 website proxy
resource "aws_api_gateway_resource" "scales_s3_site" {
  parent_id   = aws_api_gateway_rest_api.scales_api.root_resource_id
  path_part   = "{bucketkey}"
  rest_api_id = aws_api_gateway_rest_api.scales_api.id
}

resource "aws_api_gateway_method" "scales_s3_site" {
  http_method   = "GET"
  authorization = "NONE"
  resource_id   = aws_api_gateway_resource.scales_s3_site.id
  rest_api_id   = aws_api_gateway_rest_api.scales_api.id
  request_parameters = {
    "method.request.path.bucketkey" = false
  }
}

resource "aws_api_gateway_integration" "scales_s3_site" {
  http_method             = aws_api_gateway_method.scales_s3_site.http_method
  integration_http_method = aws_api_gateway_method.scales_s3_site.http_method
  resource_id             = aws_api_gateway_resource.scales_s3_site.id
  rest_api_id             = aws_api_gateway_rest_api.scales_api.id
  type                    = "HTTP_PROXY"
  uri                     = "http://${aws_s3_bucket.scales_api_website.website_endpoint}/{bucketkey}"
  request_parameters = {
    "integration.request.path.bucketkey" = "method.request.path.bucketkey"
  }
}

# Lambda function invocation
resource "aws_api_gateway_resource" "scales_api_base" {
  parent_id   = aws_api_gateway_rest_api.scales_api.root_resource_id
  path_part   = "api"
  rest_api_id = aws_api_gateway_rest_api.scales_api.id
}

resource "aws_api_gateway_resource" "scales_api_endpoint" {
  parent_id   = aws_api_gateway_resource.scales_api_base.id
  path_part   = "scales"
  rest_api_id = aws_api_gateway_rest_api.scales_api.id
}