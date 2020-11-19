using KafkaTests.Abstractions.Models;

namespace KafkaTests.Abstractions
{
    public interface IOperationStepOrderedProcessing
    {
        void Next(OperationStepDto next);
    }
}
