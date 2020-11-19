using Confluent.Kafka;
using KafkaTests.Abstractions.Models;
using KafkaTests.Abstractions.Persistence;
using KafkaTests.Producer.Contriollers.Dto;
using Newtonsoft.Json;
using System;

namespace KafkaTests.Producer.Processing
{
    public class ProducerProcessing
    {
        private readonly IOperationResultRepository _operationResultRepository;

        public ProducerProcessing(IOperationResultRepository operationResultRepository)
        {
            _operationResultRepository = operationResultRepository;
        }

        public OperationResultDto Start(CreateOperationDto dto)
        {
            var topic = "test-topic";
            var config = new ProducerConfig
            {
                BootstrapServers = "127.0.0.1:9092"
            };

            var operationId = Guid.NewGuid();

            var resultState = new OperationResultDto(operationId);
            _operationResultRepository.Set(resultState);

            var key = operationId.ToString();

            if (dto.IsOrdered)
            {
                using (var producer = new ProducerBuilder<string, string>(config).Build())
                {
                    for(var i = dto.StepNumberFrom; i <= dto.MaxStep; i++)
                    {
                        producer.Produce(topic, new Message<string, string> { Key = key, Value = ProvideMessage(key, operationId, i, dto.MaxStep) });
                    }

                    producer.Flush();
                }
            }
            else
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    for (var i = dto.StepNumberFrom; i <= dto.MaxStep; i++)
                    {
                        producer.Produce(topic, new Message<Null, string> { Value = ProvideMessage(key, operationId, i, dto.MaxStep) });
                    }

                    producer.Flush();
                }
            }

            return resultState;
        }

        private string ProvideMessage(string key, Guid operationId, int num, int maxStep)
        {
            var step = new OperationStepDto
            {
                OperationId = operationId,
                StepNumber = num,
                MaxStep = maxStep
            };

            return JsonConvert.SerializeObject(step);
        }
    }
}
