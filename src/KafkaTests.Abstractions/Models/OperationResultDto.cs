using System;

namespace KafkaTests.Abstractions.Models
{
    public class OperationResultDto
    {
        public Guid OperationId { get; set; }
        public OperationResultStatusDto Status { get; set; }
        public DateTime StartedAt { get; set; }
        public TimeSpan? Duration { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        public OperationResultDto(Guid operationId)
        {
            OperationId = operationId;
            Status = OperationResultStatusDto.Processing;
            StartedAt = DateTime.Now;
            HasError = false;
            ErrorMessage = null;
        }

        public void SetResult()
        {
            Status = OperationResultStatusDto.Success;
            Duration = new TimeSpan((DateTime.Now - StartedAt).Ticks);
        }

        public void SetError(string errorMessage)
        {
            Status = OperationResultStatusDto.Error;
            HasError = true;
            ErrorMessage = errorMessage;
            Duration = new TimeSpan((DateTime.Now - StartedAt).Ticks);
        }
    }
}
