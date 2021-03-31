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