#!/bin/bash

action=$1

if [ -z $action ]; then
    echo "want exactly one argument equal to 'create' or 'destroy'"
    exit 1
fi

staticDir="../app/static"
lambdaDir="./lambda"
lambdaExecutable="handler"

# Set Terraform environment variables
export TF_VAR_lambda_zip="$lambdaDir/$lambdaExecutable.zip"
export TF_VAR_lambda_handler=$lambdaExecutable
export TF_VAR_public_ip=$(curl -s https://checkip.amazonaws.com)
export TF_VAR_static_files_dir=$staticDir

if [ $action == "create" ]; then
    # Build Go Lambda function handler
    cd $lambdaDir
    GOOS=linux GOARCH=amd64 CGO_ENABLED=0 go build -o $lambdaExecutable "$lambdaExecutable.go"
    zip "$lambdaExecutable.zip" $lambdaExecutable
    rm -f $lambdaExecutable

    # Deploy to AWS with Terraform
    cd -
    terraform fmt || { echo 'Terraform formatting error'; exit 1; }
    terraform validate || { echo 'Terraform validation error'; exit 1; }
    terraform apply || { echo 'Terraform apply error'; exit 1; }
elif [ $action == "destroy" ]; then
    # Destroy AWS infrastructure
    terraform destroy || { echo 'Terraform destruction error', exit 1; }
else 
    echo "invalid argument: want 'create' or 'destroy', got '$action'"
    exit 1
fi