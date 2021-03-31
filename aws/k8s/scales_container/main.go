package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"

	"github.com/alexpcook/practice-music-scales/app/scales"
)

const (
	// Consider making these constants build arguments available to Docker
	containerApiRoute   string = "/api/scales"
	containerListenPort int    = 9000
)

func main() {
	http.HandleFunc(containerApiRoute, func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("content-type", "application/json")

		err := json.NewEncoder(w).Encode(scales.GetRandomScales())
		if err != nil {
			w.WriteHeader(http.StatusInternalServerError)
			return
		}

		w.WriteHeader(http.StatusOK)
	})

	log.Fatal(
		http.ListenAndServe(
			fmt.Sprintf(":%d", containerListenPort), nil,
		),
	)
}
