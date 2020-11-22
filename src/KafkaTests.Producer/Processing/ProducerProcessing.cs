using Confluent.Kafka;
using KafkaTests.Abstractions.Models;
using KafkaTests.Abstractions.Persistence;
using KafkaTests.Producer.Contriollers.Dto;
using Newtonsoft.Json;
using System;

namespace KafkaTests.Producer.Processing
{
    public class ProducerProcessing : IProducerProcessing
    {
        private readonly IOperationResultRepository _operationResultRepository;
        private readonly string _kafkaTopic;
        private readonly ProducerConfig _config;

        public ProducerProcessing(IOperationResultRepository operationResultRepository, string kafkaTopic, string kafkaUrl)
        {
            _operationResultRepository = operationResultRepository;
            _kafkaTopic = kafkaTopic;
            _config = new ProducerConfig
            {
                BootstrapServers = kafkaUrl
            };
        }

        public OperationResultDto Start(CreateOperationDto dto)
        {
            var operationId = Guid.NewGuid();

            var resultState = new OperationResultDto(operationId);
            _operationResultRepository.Set(resultState);

            var key = operationId.ToString();

            if (dto.IsOrdered)
            {
                using (var producer = new ProducerBuilder<string, string>(_config).Build())
                {
                    for(var i = dto.StepNumberFrom; i <= dto.MaxStep; i++)
                    {
                        producer.Produce(_kafkaTopic, new Message<string, string> { Key = key, Value = ProvideMessage(key, operationId, i, dto.MaxStep) });
                    }

                    producer.Flush();
                }
            }
            else
            {
                using (var producer = new ProducerBuilder<Null, string>(_config).Build())
                {
                    for (var i = dto.StepNumberFrom; i <= dto.MaxStep; i++)
                    {
                        producer.Produce(_kafkaTopic, new Message<Null, string> { Value = ProvideMessage(key, operationId, i, dto.MaxStep) });
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
