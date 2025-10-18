using GanhoDeCapital.Core.Domain.Entites;

namespace GanhoDeCapital.Core.Interfaces
{
    public interface IClientService
    {
        Task<Client?> GetClientAsync(Guid clientId);
    }
}
