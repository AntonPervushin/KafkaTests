using System;

namespace KafkaTests.Abstractions.Models
{
    public class OperationStepDto
    {
        public Guid OperationId { get; set; }
        public int StepNumber { get; set; }
        public int MaxStep { get; set; }

        public override string ToString()
        {
            return $"OrerationId: {OperationId}. StepNumber: {StepNumber}. MaxStep: {MaxStep}";
        }
    }
}
