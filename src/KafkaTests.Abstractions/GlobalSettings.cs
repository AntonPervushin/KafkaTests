using System;

namespace KafkaTests.Abstractions
{
    public class GlobalSettings
    {
        public string KafkaTopic { get; }
        public string KafkaUrl { get; }
        public string RedisUrl { get; }
        public string ClientGroup { get; }

        public GlobalSettings(string kafkaTopic, string kafkaUrl, string redisUrl, string clientGroup)
        {
            KafkaTopic = kafkaTopic;
            KafkaUrl = kafkaUrl;
            RedisUrl = redisUrl;
            ClientGroup = clientGroup;
        }

        public static GlobalSettings Config { get; private set; }
        public static void SetFromEnvironment()
        {
            Config = new GlobalSettings(
                GetFromEnvironment("KAFKA_TOPIC") ?? "test-topic",
                GetFromEnvironment("KAFKA_URL") ?? "localhost:9092",
                GetFromEnvironment("REDIS_URL") ?? "localhost:6379",
                GetFromEnvironment("CLIENT_GROUP") ?? "service-1");
        }

        private static string? GetFromEnvironment(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
