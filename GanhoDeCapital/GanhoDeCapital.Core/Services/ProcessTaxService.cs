using GanhoDeCapital.Core.Domain.Constants;
using GanhoDeCapital.Core.Domain.DTOs;
using GanhoDeCapital.Core.Domain.Entites;
using GanhoDeCapital.Core.Domain.Entities;
using GanhoDeCapital.Core.Interfaces;

namespace GanhoDeCapital.Core.Services
{
    public class ProcessTaxService
    {
        private readonly ITaxCalculationService _taxCalculationService;
        private readonly IClientService _clientService;
        private readonly IOperationRepository _operationRepository;

        public ProcessTaxService(
            ITaxCalculationService taxCalculationService,
            IClientService clientService,
            IOperationRepository operationRepository)
        {
            _taxCalculationService = taxCalculationService;
            _clientService = clientService;
            _operationRepository = operationRepository;
        }

        public async Task<List<OperationResponse>> ProcessOperationsAsync(
            List<OperationRequest> requests)
        {
            var responses = new List<OperationResponse>();

            foreach (var request in requests)
            {
                var response = await ProcessSingleOperationAsync(request);
                responses.Add(response);
            }

            return responses;
        }

        private async Task<OperationResponse> ProcessSingleOperationAsync(
            OperationRequest request)
        {
            var response = new OperationResponse
            {
                OperationId = request.OperationId,
                ClientId = request.ClientId
            };

            if (!request.ClientId.HasValue)
            {
                response.Status = OperationStatus.EnrichmentError;
                await SaveOperationAsync(request, response);
                return response;
            }
                        
            var client = await _clientService.GetClientAsync(request.ClientId.Value);

            if (client == null)
            {
                response.Status = OperationStatus.EnrichmentError;
                await SaveOperationAsync(request, response);
                return response;
            }

            response.ClientName = client.ClientName;
            response.ClientCpf = client.ClientCpf;
                        
            var transactions = request.Operations
                .Select(op => new Transaction
                {
                    Operation = op.Operation,
                    UnitCost = op.UnitCost,
                    Quantity = op.Quantity
                })
                .ToList();
            
            decimal tax = _taxCalculationService.CalculateTax(transactions, out string status);

            response.Tax = status == OperationStatus.CalculatedSuccessfully ? tax : null;
            response.Status = status;

            await SaveOperationAsync(request, response);

            return response;
        }

        private async Task SaveOperationAsync(OperationRequest request,OperationResponse response)
        {
            var operation = new Operation
            {
                OperationId = request.OperationId,
                ClientId = request.ClientId,
                ClientName = response.ClientName,
                ClientCpf = response.ClientCpf,
                Tax = response.Tax,
                Status = response.Status,
                Transactions = request.Operations
                    .Select(op => new Transaction
                    {
                        Operation = op.Operation,
                        UnitCost = op.UnitCost,
                        Quantity = op.Quantity
                    })
                    .ToList()
            };

            await _operationRepository.SaveAsync(operation);
        }
    }
}
