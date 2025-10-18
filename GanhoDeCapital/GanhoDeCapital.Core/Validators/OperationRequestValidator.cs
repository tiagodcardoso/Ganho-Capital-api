using GanhoDeCapital.Core.Domain.Constants;
using GanhoDeCapital.Core.Domain.DTOs;

namespace GanhoDeCapital.Core.Validators
{
    public class OperationRequestValidator
    {
        public List<string> Validate(List<OperationRequest> requests)
        {
            var errors = new List<string>();

            if (requests == null || requests.Count == 0)
            {
                errors.Add("A lista de operações não pode ser vazia.");
                return errors;
            }

            foreach (var request in requests)
            {
                ValidateRequest(request, errors);
            }

            return errors;
        }

        private void ValidateRequest(OperationRequest request, List<string> errors)
        {
            if (request.OperationId <= 0)
            {
                errors.Add($"O operation-id deve ser maior que zero.");
            }

            if (request.Operations == null || request.Operations.Count == 0)
            {
                errors.Add($"A lista de transações do operation-id {request.OperationId} não pode ser vazia.");
                return;
            }

            foreach (var transaction in request.Operations)
            {
                ValidateTransaction(transaction, request.OperationId, errors);
            }
        }

        private void ValidateTransaction(
            TransactionDto transaction,
            long operationId,
            List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(transaction.Operation))
            {
                errors.Add($"O tipo de operação não pode ser vazio (operation-id: {operationId}).");
            }
            else if (transaction.Operation != OperationType.Buy &&
                     transaction.Operation != OperationType.Sell)
            {
                errors.Add($"Tipo de operação inválido: {transaction.Operation}. Use 'buy' ou 'sell' (operation-id: {operationId}).");
            }

            if (transaction.UnitCost <= 0)
            {
                errors.Add($"O preço unitário deve ser maior que zero (operation-id: {operationId}).");
            }

            if (transaction.Quantity <= 0)
            {
                errors.Add($"A quantidade deve ser maior que zero (operation-id: {operationId}).");
            }
        }
    }
}
