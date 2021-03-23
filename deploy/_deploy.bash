#!/bin/bash

appDir="../app"
apiDir="$appDir/api"
staticDir="$appDir/static"

lambdaExecutable="handler"

# Build Go Lambda function handler...
cd $apiDir
GOOS=linux GOARCH=amd64 go build -o $lambdaExecutable "$lambdaExecutable.go"
zip "$lambdaExecutable.zip" $lambdaExecutable
rm -f $lambdaExecutable

# Deploy to AWS with Terraform...
cd -

export TF_VAR_lambda_zip="$apiDir/$lambdaExecutable.zip"
export TF_VAR_lambda_handler=$lambdaExecutable
export TF_VAR_public_ip=$(curl -s https://checkip.amazonaws.com)
export TF_VAR_static_files_dir=$staticDir

terraform fmt || { echo 'Terraform formatting error'; exit 1; }
terraform validate || { echo 'Terraform validation error'; exit 1; }
terraform apply || { echo 'Terraform apply error'; exit 1; }
