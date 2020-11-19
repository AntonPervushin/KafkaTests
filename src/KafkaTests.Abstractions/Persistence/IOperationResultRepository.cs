using KafkaTests.Abstractions.Models;
using System;

namespace KafkaTests.Abstractions.Persistence
{
    public interface IOperationResultRepository : IDisposable
    {
        public OperationResultDto Get(Guid id);
        public void Set(OperationResultDto dto);
    }
}
