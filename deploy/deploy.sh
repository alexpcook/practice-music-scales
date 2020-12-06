#!/bin/bash

# Build with Go
cd ../app/api
GOOS=linux GOARCH=amd64 go build -o handler handler.go
zip handler.zip handler
rm -f handler

# Deploy to AWS
cd -
pulumi up
