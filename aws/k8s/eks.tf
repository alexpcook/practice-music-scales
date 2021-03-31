resource "aws_eks_cluster" "scales" {
  name     = var.name_prefix
  role_arn = aws_iam_role.scales.arn

  vpc_config {
    subnet_ids = data.aws_subnet_ids.default.ids
  }

  depends_on = [
    aws_iam_role_policy_attachment.cluster_policy,
    aws_iam_role_policy_attachment.resource_controller,
  ]
}

resource "aws_iam_role" "scales" {
  name = join("-", [var.name_prefix, "eks", "cluster"])

  assume_role_policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "eks.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
POLICY
}

resource "aws_iam_role_policy_attachment" "cluster_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSClusterPolicy"
  role       = aws_iam_role.scales.name
}

resource "aws_iam_role_policy_attachment" "resource_controller" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSVPCResourceController"
  role       = aws_iam_role.scales.name
}