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

variable "static_files_dir" {
  description = "The base directory of the static website files to deploy to S3."
  type        = string
}

variable "static_file_mime_types" {
  description = "The MIME type mapping to apply to static website files in the S3 bucket."
  type        = map(string)
  default = {
    html = "text/html"
    css  = "text/css"
    js   = "text/javascript"
  }
}