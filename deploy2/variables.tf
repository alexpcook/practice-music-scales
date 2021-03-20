variable "aws_profile" {
  description = "The AWS profile to deploy to."
  type        = string
}

variable "aws_region" {
  description = "The AWS region to deploy to."
  type        = string
}

variable "name_prefix" {
  description = "The naming prefix to apply to AWS resources."
  type        = string
}

variable "lambda_zip" {
  description = "The zip archive containing the Lambda function code."
  type        = string
}

variable "lambda_handler" {
  description = "The executable from the 'go build' command for the Lambda function to execute."
  type        = string
}

variable "public_ip" {
  description = "The public IP address to allow access to the S3 bucket and API gateway."
  type        = string
  sensitive   = true
}