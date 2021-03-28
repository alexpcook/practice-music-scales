# practice-music-scales

I play piano and wanted to devise a way to make practicing scales more fun and challenging. One way to increase the level of difficulty is to play all the major and minor scales in a random order instead of following the circle of fifths. This forces you to better learn each scale in isolation rather than relying on the familiar pattern of adding an additional sharp or flat as you go around the circle of fifths.

To accomplish this goal, this repository contains:

1. A Go package of APIs to return major and minor scales in a random order (`app/scales`).
2. A set of static website files to call the API and display the result (`app/static`).
3. Various options to deploy the code. Currently, the only option is a serverless deployment on AWS using API gateway, Lambda, and S3 (`aws/serverless`). More deployment options will become available in the future.

## Deployment options

### AWS Serverless

This option invokes a Lambda function written in Go via an API gateway endpoint. The Go handler returns a JSON response with both major and minor scales in a random order for practice. An S3 bucket (also proxied via the API Gateway) contains the static website files to display the result of the Lambda function to the user. The AWS resources are deployed and managed with a Terraform stack and a Bash helper script.

See [GitHub AWS Lambda Go repo](https://github.com/aws/aws-lambda-go) for more information. The Bash script builds the Go executable and zips it up prior to handling the Terraform deployment:

```bash
cd aws/serverless
./_deploy.bash create
```

To destroy the deployment, run the bash script again with a destroy argument:

```bash
./_deploy.bash destroy
```
