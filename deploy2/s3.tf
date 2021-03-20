resource "aws_s3_bucket" "scales_api_website" {
  bucket_prefix = join("-", [var.name_prefix, "website"])
}