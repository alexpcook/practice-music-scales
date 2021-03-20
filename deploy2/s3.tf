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

resource "aws_s3_bucket_object" "scales_api_website_files" {
  for_each = fileset(var.static_files_dir, "*/*")

  bucket       = aws_s3_bucket.scales_api_website.id
  key          = split("/", each.key)[1]
  source       = join("/", [var.static_files_dir, each.value])
  etag         = filemd5(join("/", [var.static_files_dir, each.value]))
  content_type = lookup(var.static_file_mime_types, split(".", each.key)[1], "text/plain")
}