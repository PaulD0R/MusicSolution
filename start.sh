#!/bin/bash

NETWORK_NAME="music-solution-network"
REDIS_FILE="Redis/compose.yaml"
GRAFANA_FILE="Grafana/compose.yaml" 
KAFKA_FILE="Kafka/compose.yaml" 
MUSIC_SERVICE_FILE="MusicService/compose.yaml"
USER_SERVICE_FILE="UserService/compose.yaml"
COMMENT_SERVICE_FILE="CommentService/compose.yaml"
YARP_FILE="YARP/compose.yaml"

KAFKA_CONTAINER_NAME="kafka-server"
BOOTSTRAP_SERVER="localhost:9092"
TOPICS=("music-created" "music-deleted" "person-created" "person-deleted" "person-updated")

if [ ! "$(docker network ls | grep -w $NETWORK_NAME)" ]; then
  docker network create $NETWORK_NAME
fi

docker compose -f $REDIS_FILE -f $GRAFANA_FILE -f $KAFKA_FILE -f $MUSIC_SERVICE_FILE -f $USER_SERVICE_FILE -f $COMMENT_SERVICE_FILE -f $YARP_FILE down --remove-orphans

docker compose -f $REDIS_FILE up -d
docker compose -f $GRAFANA_FILE up -d
docker compose -f $KAFKA_FILE up -d

until docker exec $KAFKA_CONTAINER_NAME kafka-topics.sh --bootstrap-server $BOOTSTRAP_SERVER --list &> /dev/null; do
  sleep 2
done

for TOPIC in "${TOPICS[@]}"; do
  docker exec $KAFKA_CONTAINER_NAME \
    kafka-topics.sh --create \
    --if-not-exists \
    --bootstrap-server $BOOTSTRAP_SERVER \
    --topic $TOPIC 
done

docker compose -f $MUSIC_SERVICE_FILE up -d
docker compose -f $USER_SERVICE_FILE up -d
docker compose -f $COMMENT_SERVICE_FILE up -d
docker compose -f $YARP_FILE up -d

docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"