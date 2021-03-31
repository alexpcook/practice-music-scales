FROM alpine:latest

ARG go_binary

WORKDIR /app

COPY $go_binary .

ENV GO_BINARY=$go_binary

EXPOSE 9000/tcp

ENTRYPOINT ["/bin/ash", "-c", "./$GO_BINARY"]
