# General setup
resource "aws_api_gateway_rest_api" "scales_api" {
  name = join("-", [var.name_prefix, "gateway"])
}

resource "aws_api_gateway_deployment" "scales_api" {
  rest_api_id = aws_api_gateway_rest_api.scales_api.id

  depends_on = [
    aws_api_gateway_method.scales_s3_site,
    aws_api_gateway_method.scales_api_endpoint,
    aws_api_gateway_integration.scales_s3_site,
    aws_api_gateway_integration.scales_api_endpoint,
  ]
}

resource "aws_api_gateway_stage" "scales_api" {
  deployment_id = aws_api_gateway_deployment.scales_api.id
  rest_api_id   = aws_api_gateway_rest_api.scales_api.id
  stage_name    = "dev"
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

resource "aws_api_gateway_method" "scales_api_endpoint" {
  http_method   = "GET"
  authorization = "NONE"
  resource_id   = aws_api_gateway_resource.scales_api_endpoint.id
  rest_api_id   = aws_api_gateway_rest_api.scales_api.id
}

resource "aws_api_gateway_integration" "scales_api_endpoint" {
  http_method             = aws_api_gateway_method.scales_api_endpoint.http_method
  integration_http_method = "POST"
  resource_id             = aws_api_gateway_resource.scales_api_endpoint.id
  rest_api_id             = aws_api_gateway_rest_api.scales_api.id
  type                    = "AWS_PROXY"
  uri                     = aws_lambda_function.scales_api.invoke_arn
}