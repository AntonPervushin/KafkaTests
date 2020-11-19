using KafkaTests.Abstractions.Models;
using KafkaTests.Abstractions.Persistence;
using KafkaTests.Producer.Contriollers.Dto;
using KafkaTests.Producer.Processing;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KafkaTests.Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OperationController : ControllerBase
    {
        private readonly IOperationResultRepository _operationResultRepository;

        public OperationController(IOperationResultRepository operationResultRepository)
        {
            _operationResultRepository = operationResultRepository;
        }

        [HttpGet]
        [Route("{operationId}")]
        public OperationResultDto Get([FromRoute] Guid operationId)
        {
            return _operationResultRepository.Get(operationId);
        }

        [HttpPost]
        public OperationResultDto Post(CreateOperationDto dto)
        {
            var producer = new ProducerProcessing(_operationResultRepository);
            return producer.Start(dto);        }
    }
}
