#!/bin/bash

docker compose -f Redis/compose.yaml down --remove-orphans
docker compose -f Grafana/compose.yaml down --remove-orphans
docker compose -f Kafka/compose.yaml down --remove-orphans
docker compose -f MusicService/compose.yaml down --remove-orphans
docker compose -f UserService/compose.yaml down --remove-orphans
docker compose -f YARP/compose.yaml down --remove-orphans