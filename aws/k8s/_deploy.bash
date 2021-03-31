#!/bin/bash

# Build the Nginx static website container
staticContainerSourceDir="../../app/static"
staticContainerBuildDir="static_container"
staticContainerFilesSubdir="static"

rm -rf $staticContainerBuildDir/$staticContainerFilesSubdir

cp -vR $staticContainerSourceDir $staticContainerBuildDir || { echo 'Failed to copy static files to container directory'; exit 1; }
docker build --build-arg static_files_dir=$staticContainerFilesSubdir -t practice-music-scales-static $staticContainerBuildDir

rm -rf $staticContainerBuildDir/$staticContainerFilesSubdir

# Build the Go microservice container
apiContainerBuildDir="scales_container"
goBinary="main"

cd $apiContainerBuildDir
GOOS=linux GOARCH=amd64 CGO_ENABLED=0 go build -o $goBinary
docker build --build-arg go_binary=$goBinary -t practice-music-scales-api .

rm -rf $goBinary
cd -
