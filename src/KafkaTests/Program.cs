using System;
using System.Collections.Generic;
using Confluent.Kafka;


namespace KafkaTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
                GroupId = "service-1",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(new[] { "test-topic" });

                while (true)
                {
                    var consumeResult = consumer.Consume(1000);

                    if(consumeResult != null)
                    {

                    }

                }

                consumer.Close();
            }

        }
    }
}
