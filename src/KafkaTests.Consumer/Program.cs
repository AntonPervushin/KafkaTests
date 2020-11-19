using Confluent.Kafka;
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
            var cts = new CancellationTokenSource();
            var ctsToken = cts.Token;


            var workerTask = Task.Run(() => DoWork(ctsToken), ctsToken).ContinueWith(t => CheckComplete(t));
            var waiterTask = Task.Run(() => CheckExit(cts), ctsToken).ContinueWith(t => CheckComplete(t));

            try
            {
                Task.WaitAny(new[] { workerTask, waiterTask });
            }
            finally
            {
                cts.Dispose();
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void DoWork(CancellationToken cancellationToken)
        {
            var topic = "test-topic";
            var config = new ConsumerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
                GroupId = "service-1",
                AutoOffsetReset = AutoOffsetReset.Latest
            };

            cancellationToken.ThrowIfCancellationRequested();

            using (var processing = new OperationStepOrderedProcessing(new RedisOperationResultRepository()))
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

        private static void CheckExit(CancellationTokenSource cts)
        {
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q) cts.Cancel();
            }
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
