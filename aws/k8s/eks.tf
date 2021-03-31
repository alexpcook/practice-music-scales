resource "aws_eks_cluster" "scales" {
  name     = var.name_prefix
  role_arn = aws_iam_role.scales_manager.arn

  vpc_config {
    subnet_ids = data.aws_subnet_ids.default.ids
  }

  depends_on = [
    aws_iam_role_policy_attachment.cluster_policy,
    aws_iam_role_policy_attachment.resource_controller,
  ]
}

resource "aws_iam_role" "scales_manager" {
  name = join("-", [var.name_prefix, "eks", "cluster", "manager"])

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
  role       = aws_iam_role.scales_manager.name
}

resource "aws_iam_role_policy_attachment" "resource_controller" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSVPCResourceController"
  role       = aws_iam_role.scales_manager.name
}

resource "aws_eks_node_group" "scales" {
  cluster_name    = aws_eks_cluster.scales.name
  node_group_name = join("-", [var.name_prefix, "nodes"])
  node_role_arn   = aws_iam_role.scales_worker.arn
  subnet_ids      = data.aws_subnet_ids.default.ids

  scaling_config {
    desired_size = 2
    max_size     = 3
    min_size     = 1
  }

  depends_on = [
    aws_iam_role_policy_attachment.worker_node,
    aws_iam_role_policy_attachment.cni,
    aws_iam_role_policy_attachment.ecr_readonly,
  ]
}

resource "aws_iam_role" "scales_worker" {
  name = join("-", [var.name_prefix, "eks", "cluster", "worker"])

  assume_role_policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "ec2.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
POLICY
}

resource "aws_iam_role_policy_attachment" "worker_node" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSWorkerNodePolicy"
  role       = aws_iam_role.scales_worker.name
}

resource "aws_iam_role_policy_attachment" "cni" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKS_CNI_Policy"
  role       = aws_iam_role.scales_worker.name
}

resource "aws_iam_role_policy_attachment" "ecr_readonly" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly"
  role       = aws_iam_role.scales_worker.name
}