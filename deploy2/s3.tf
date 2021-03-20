resource "aws_s3_bucket" "scales_api_website" {
  bucket_prefix = join("-", [var.name_prefix, "website"])

  website {
    index_document = "site.html"
  }
}

resource "aws_s3_bucket_policy" "allow_public_read" {
  bucket = aws_s3_bucket.scales_api_website.id

  policy = jsonencode({
    Version = "2012-10-17"
    Id      = "AllowPublicRead"
    Statement = [
      {
        Sid       = "AllowPublicReadFromIP"
        Effect    = "Allow"
        Principal = "*"
        Action    = "s3:GetObject"
        Resource  = "${aws_s3_bucket.scales_api_website.arn}/*"
        Condition = {
          IpAddress = {
            "aws:SourceIp" = var.public_ip
          }
        }
      }
    ]
  })
}