using KafkaTests.Abstractions;
using KafkaTests.Abstractions.Models;
using KafkaTests.Abstractions.Persistence;
using System;

namespace KafkaTests.Implementations
{
    public class OperationStepOrderedProcessing : IOperationStepOrderedProcessing, IDisposable
    {
        private readonly IOperationResultRepository _operationResultRepository;
        private OperationStepDto _currentStep;

        public OperationStepOrderedProcessing(IOperationResultRepository operationResultRepository)
        {
            _operationResultRepository = operationResultRepository;
        }

        public void Next(OperationStepDto next)
        {
            if (next == null) throw new ArgumentNullException("next");

            var resultState = _operationResultRepository.Get(next.OperationId);

            if (resultState?.Status != OperationResultStatusDto.Processing) return;

            if (_currentStep == null || _currentStep.OperationId != next.OperationId || next.StepNumber == _currentStep.StepNumber + 1)
            {
                _currentStep = next;

                if (next.StepNumber == next.MaxStep)
                {
                    resultState.SetResult();
                    _operationResultRepository.Set(resultState);

                    _currentStep = null;
                }
            }
            else
            {
                resultState.SetError($"Wrong processing. Current: {_currentStep}. Next: {next}");

                _operationResultRepository.Set(resultState);

                _currentStep = null;
            }
        }

        public void Dispose()
        {
            _operationResultRepository.Dispose();
        }
    }
}
