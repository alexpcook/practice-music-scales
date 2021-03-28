package main

import (
	"encoding/json"
	"errors"

	"github.com/alexpcook/practice-music-scales/app/scales"
	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-lambda-go/lambda"
)

func randomScalesHandler(request events.APIGatewayProxyRequest) (events.APIGatewayProxyResponse, error) {
	jsonData, err := json.Marshal(scales.GetRandomScales())
	if err != nil {
		errMsg := "error forming json response body"
		return events.APIGatewayProxyResponse{
			StatusCode: 500,
			Body:       errMsg,
		}, errors.New(errMsg)
	}

	return events.APIGatewayProxyResponse{
		StatusCode: 200,
		Body:       string(jsonData),
	}, nil
}

func main() {
	lambda.Start(randomScalesHandler)
}
