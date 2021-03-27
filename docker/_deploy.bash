#!/bin/bash

staticFilesDir="../app/static"
buildDir="static_files"

# Move static site files to Docker directory to build the image
rm -rf $buildDir
mkdir $buildDir
find $staticFilesDir -name site.* | xargs -I % cp % $buildDir

# Build the Docker image
docker build --build-arg static_files_dir=$buildDir -t practice-music-scales .

# Clean up
rm -rf $buildDir
