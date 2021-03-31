FROM golang:latest AS builder

ARG api_dir

WORKDIR /build

COPY $api_dir .

RUN GOOS=linux GOARCH=amd64 CGO_ENABLED=0 go build -o practice-music-scales

FROM alpine:latest AS runner

WORKDIR /app

COPY --from=builder ./practice-music-scales .

EXPOSE 80/tcp

ENTRYPOINT ["./practice-music-scales"]
