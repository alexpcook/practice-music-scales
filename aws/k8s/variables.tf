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

variable "ecr_repository_names" {
  description = "The names of the ECR repositories to create. This should match the Docker image repository tags."
  type        = list(string)
  default     = ["practice-music-scales-api", "practice-music-scales-static"]
}