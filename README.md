# practice-music-scales

I play piano and wanted to devise a way to make practicing scales more fun and challenging. One way to increase the level of difficulty is to play all the major and minor scales in a random order instead of following the circle of fifths. This forces you to better learn each scale in isolation rather than relying on the familiar pattern of adding an additional sharp or flat as you go around the circle of fifths.

To accomplish this goal, this repository contains an AWS Lambda function in Go invoked via an API Gateway endpoint. The Go handler returns a JSON response with both major and minor scales in a random order for practice. An S3 bucket (also proxied via the API Gateway) contains static HTML, CSS, and JavaScript to display the result of the Lambda function to the user. The AWS resources are deployed and managed with a Terraform stack.

## How to build the AWS Lambda Go function

See [GitHub AWS Lambda Go repo](https://github.com/aws/aws-lambda-go) for more information. The bash script in the deploy directory builds the Go executable and zips it up prior to handling the Terraform deployment:

```
cd deploy
./_deploy.bash create
```

To destroy the deployment, run the bash script again with a destroy argument:

```
cd deploy
./_deploy.bash destroy
```
