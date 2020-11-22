using KafkaTests.Abstractions.Models;
using KafkaTests.Producer.Contriollers.Dto;

namespace KafkaTests.Producer.Processing
{
    public interface IProducerProcessing
    {
        OperationResultDto Start(CreateOperationDto dto);
    }
}
