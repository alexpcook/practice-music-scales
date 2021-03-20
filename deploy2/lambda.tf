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