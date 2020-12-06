# How to build the AWS Lambda Go function

See [GitHub AWS Lambda Go repo](https://github.com/aws/aws-lambda-go) for more information

1. At the terminal in this directory, run `GOOS=linux GOARCH=amd64 go build -o dist/handler handler.go` to build a Linux executable
2. Run `zip dist/handler.zip dist/handler` to create a zip file of the executable
