#!/bin/bash

# Build the static files container
staticContainerSourceDir="../../app/static"
staticContainerBuildDir="static_container"
staticContainerFilesSubdir="static"

rm -rf $staticContainerBuildDir/$staticContainerFilesSubdir

cp -vR $staticContainerSourceDir $staticContainerBuildDir || { echo 'Failed to copy static files to container directory'; exit 1; }
docker build --build-arg static_files_dir=$staticContainerFilesSubdir -t practice-music-scales-static $staticContainerBuildDir

rm -rf $staticContainerBuildDir/$staticContainerFilesSubdir

# Build the Go microservice container
