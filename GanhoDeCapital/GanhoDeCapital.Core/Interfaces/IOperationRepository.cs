using GanhoDeCapital.Core.Domain.Entites;

namespace GanhoDeCapital.Core.Interfaces
{
    public interface IOperationRepository
    {
        Task<Operation> SaveAsync(Operation operation);
        Task<Operation?> GetByIdAsync(long operationId);
        Task<IEnumerable<Operation>> GetByClientIdAsync(Guid clientId);
        Task<IEnumerable<Operation>> GetAllAsync();
    }
}
