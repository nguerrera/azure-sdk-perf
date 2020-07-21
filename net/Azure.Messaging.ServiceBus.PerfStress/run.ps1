#!/usr/bin/env pwsh

docker run -it --rm --network host -e SERVICEBUS_CONNECTION_STRING -e SERVICEBUS_QUEUE azure-messaging-servicebus-perfstress-net @args
