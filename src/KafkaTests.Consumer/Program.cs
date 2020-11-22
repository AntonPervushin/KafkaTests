using Confluent.Kafka;
using KafkaTests.Abstractions;
using KafkaTests.Abstractions.Models;
using KafkaTests.Implementations;
using KafkaTests.Implementations.Persistence;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaTests.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalSettings.SetFromEnvironment();

            var cts = new CancellationTokenSource();
            var ctsToken = cts.Token;


            var workerTask = Task.Run(() => DoWork(ctsToken), ctsToken).ContinueWith(t => CheckComplete(t));

            try
            {
                Task.WaitAny(new[] { workerTask });
            }
            finally
            {
                cts.Dispose();
            }
        }

        private static void DoWork(CancellationToken cancellationToken)
        {
            var topic = GlobalSettings.Config.KafkaTopic;
            var config = new ConsumerConfig
            {
                BootstrapServers = GlobalSettings.Config.KafkaUrl,
                GroupId = GlobalSettings.Config.ClientGroup,
                AutoOffsetReset = AutoOffsetReset.Latest
            };

            Console.WriteLine($"Config: {JsonConvert.SerializeObject(GlobalSettings.Config)}");

            cancellationToken.ThrowIfCancellationRequested();

            using (var processing = new OperationStepOrderedProcessing(new RedisOperationResultRepository(GlobalSettings.Config.RedisUrl)))
            {
                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    consumer.Subscribe(new[] { topic });
                    while (true)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            consumer.Close();
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        var consumeResult = consumer.Consume(100);

                        if (consumeResult != null)
                        {
                            if (TryParseJson(consumeResult.Message.Value, out OperationStepDto data))
                            {
                                processing.Next(data);
                            }
                            else
                            {
                                Console.WriteLine($"Unknown message format {consumeResult.Message.Value}");
                            }
                        }
                    }
                }
            }
        }

        private static bool TryParseJson<T>(string @this, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(@this, settings);
            return success;
        }

        private static void CheckComplete(Task task)
        {
            if (task.IsCanceled)
            {
                Console.WriteLine($"Operation cancelled");
                return;
            }

            if (task.IsFaulted)
            {
                var exception = task.Exception?.InnerException ?? task.Exception;
                Console.WriteLine($"Exception thrown: {exception.Message}");
            }
        }
    }
}
