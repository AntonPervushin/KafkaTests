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
        private readonly IProducerProcessing _processing;

        public OperationController(IOperationResultRepository operationResultRepository, IProducerProcessing processing)
        {
            _operationResultRepository = operationResultRepository;
            _processing = processing;
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
            return _processing.Start(dto);        }
    }
}
