#!/bin/bash

# Build with Go
cd lambda
GOOS=linux GOARCH=amd64 go build -o dist/handler handler.go
zip dist/handler.zip dist/handler

# Deploy to AWS
cd ..
pulumi up -C ./pulumi
