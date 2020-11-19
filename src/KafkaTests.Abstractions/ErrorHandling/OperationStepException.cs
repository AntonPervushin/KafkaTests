using System;

namespace KafkaTests.Abstractions.ErrorHandling
{
    public class OperationStepException : Exception
    {
        public OperationStepException(string message)
            : base(message) { }
    }
}
