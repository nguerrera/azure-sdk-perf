#!/usr/bin/env pwsh

docker run -it --rm --network host -e STORAGE_CONNECTION_STRING -v ${PWD}:/app --entrypoint bash python
