namespace KafkaTests.Producer.Contriollers.Dto
{
    public class CreateOperationDto
    {
        public int StepNumberFrom { get; set; }
        public int MaxStep { get; set; }
        public bool IsOrdered { get; set; }
    }
}
