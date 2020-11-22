FROM ubuntu:20.04

RUN  apt-get update && apt-get install -y redis-server

RUN apt-get update && \
    apt-get install -y openjdk-11-jre-headless && \
    apt-get clean;

RUN mkdir /usr/src/kafka

COPY ./kafka_2.13-2.8.0 /usr/src/kafka

COPY ./start.sh /usr/src/start.sh

EXPOSE      6379
EXPOSE      9092

CMD ["bash", "/usr/src/start.sh"]
