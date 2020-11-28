package main

import (
	"encoding/json"
	"errors"
	"math/rand"
	"time"

	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-lambda-go/lambda"
)

// Scale is a type representation of a musical scale as a string
type Scale string

// Major scale constants
const (
	CMaj  Scale = "C Major"
	GMaj  Scale = "G Major"
	DMaj  Scale = "D Major"
	AMaj  Scale = "A Major"
	EMaj  Scale = "E Major"
	BMaj  Scale = "B Major"
	FsMaj Scale = "F Sharp Major"
	CsMaj Scale = "C Sharp Major"
	FMaj  Scale = "F Major"
	BbMaj Scale = "B Flat Major"
	EbMaj Scale = "E Flat Major"
	AbMaj Scale = "A Flat Major"
	DbMaj Scale = "D Flat Major"
	GbMaj Scale = "G Flat Major"
	CbMaj Scale = "C Flat Major"
)

// Minor scale constants
const (
	AMin  Scale = "A Minor"
	EMin  Scale = "E Minor"
	BMin  Scale = "B Minor"
	FsMin Scale = "F Sharp Minor"
	CsMin Scale = "C Sharp Minor"
	GsMin Scale = "G Sharp Minor"
	DsMin Scale = "D Sharp Minor"
	AsMin Scale = "A Sharp Minor"
	DMin  Scale = "D Minor"
	GMin  Scale = "G Minor"
	CMin  Scale = "C Minor"
	FMin  Scale = "F Minor"
	BbMin Scale = "B Flat Minor"
	EbMin Scale = "E Flat Minor"
	AbMin Scale = "A Flat Minor"
)

// Scale slices
var (
	MajorScales []Scale = []Scale{
		CMaj, GMaj, DMaj, AMaj, EMaj, BMaj, FsMaj, CsMaj,
		FMaj, BbMaj, EbMaj, AbMaj, DbMaj, GbMaj, CbMaj,
	}
	MinorScales []Scale = []Scale{
		AMin, EMin, BMin, FsMin, CsMin, GsMin, DsMin, AsMin,
		DMin, GMin, CMin, FMin, BbMin, EbMin, AbMin,
	}
)

func shuffleScales(scales []Scale) {
	rand.Seed(time.Now().UnixNano())
	rand.Shuffle(len(scales), func(i, j int) {
		scales[i], scales[j] = scales[j], scales[i]
	})
}

func scales(request events.APIGatewayProxyRequest) (events.APIGatewayProxyResponse, error) {
	shuffleScales(MajorScales)
	shuffleScales(MinorScales)

	jsonData, err := json.Marshal(map[string][]Scale{
		"majorScales": MajorScales,
		"minorScales": MinorScales,
	})
	if err != nil {
		return events.APIGatewayProxyResponse{
			StatusCode: 500,
			Body:       "Error forming json response body",
		}, errors.New("Error forming json response body")
	}

	return events.APIGatewayProxyResponse{
		StatusCode: 200,
		Body:       string(jsonData),
	}, nil
}

func main() {
	lambda.Start(scales)
}
