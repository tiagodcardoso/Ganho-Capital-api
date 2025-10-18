using GanhoDeCapital.Core.Domain.Entites;
using GanhoDeCapital.Core.Interfaces;
using System.Collections.Concurrent;

namespace GanhoDeCapital.Infra.Repositories
{
    public class InMemoryOperationRepository : IOperationRepository
    {
        private readonly ConcurrentDictionary<long, Operation> _operations = new();

        public Task<Operation> SaveAsync(Operation operation)
        {
            operation.CreatedAt = DateTime.UtcNow;
            operation.ProcessedAt = DateTime.UtcNow;

            _operations.AddOrUpdate(
                operation.OperationId,
                operation,
                (key, oldValue) => operation
            );

            return Task.FromResult(operation);
        }

        public Task<Operation?> GetByIdAsync(long operationId)
        {
            _operations.TryGetValue(operationId, out var operation);
            return Task.FromResult(operation);
        }

        public Task<IEnumerable<Operation>> GetByClientIdAsync(Guid clientId)
        {
            var operations = _operations.Values
                .Where(o => o.ClientId == clientId)
                .OrderByDescending(o => o.CreatedAt);

            return Task.FromResult(operations.AsEnumerable());
        }

        public Task<IEnumerable<Operation>> GetAllAsync()
        {
            var operations = _operations.Values
                .OrderByDescending(o => o.CreatedAt);

            return Task.FromResult(operations.AsEnumerable());
        }
    }
}
