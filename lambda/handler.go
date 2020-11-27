package main

import (
	"github.com/aws/aws-lambda-go/lambda"
)

func scales() (string, error) {
	return "Return scales here", nil
}

func main() {
	lambda.Start(scales)
}
