#!/bin/bash
  
set -m

kafka_topic=${KAFKA_TOPIC:-test-topic}
kafka_partitions=${KAFKA_PARTITIONS:-4}

/usr/bin/redis-server --protected-mode no &

cd /usr/src/kafka

./bin/zookeeper-server-start.sh config/zookeeper.properties &

./bin/kafka-server-start.sh config/server.properties &

./bin/kafka-topics.sh --create --topic "$kafka_topic" --partitions "$kafka_partitions" --bootstrap-server localhost:9092 &

fg 3


