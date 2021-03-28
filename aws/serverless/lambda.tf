resource "aws_iam_role" "scales_api_role" {
  name = join("-", [var.name_prefix, "lambda-execution-role"])

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "lambda.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}

resource "aws_lambda_function" "scales_api" {
  filename         = var.lambda_zip
  source_code_hash = filebase64sha256(var.lambda_zip)

  function_name = join("-", [var.name_prefix, "function"])
  role          = aws_iam_role.scales_api_role.arn

  runtime = "go1.x"
  handler = var.lambda_handler
}

resource "aws_lambda_permission" "scales_api" {
  statement_id  = "AllowAPIGatewayInvocation"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.scales_api.arn
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.scales_api.execution_arn}/*/GET${aws_api_gateway_resource.scales_api_endpoint.path}"
}