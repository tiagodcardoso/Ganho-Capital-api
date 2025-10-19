using GanhoDeCapital.Core.Domain.Entites;
using GanhoDeCapital.Core.Interfaces;

namespace GanhoDeCapital.Infra.Services
{
    public class MockClientService : IClientService
    {
        private readonly Dictionary<Guid, Client> _clients = new()
        {
            {
                Guid.Parse("41a23999-37f0-49d9-b9fb-5ac823fa4cb3"),
                new Client
                {
                    ClientId = Guid.Parse("41a23999-37f0-49d9-b9fb-5ac823fa4cb3"),
                    ClientName = "JOAO SILVA",
                    ClientCpf = "111.111.111-11"
                }
            },
            {
                Guid.Parse("c3a2f999-37f0-49d9-b9fb-5ac823fa4cb3"),
                new Client
                {
                    ClientId = Guid.Parse("c3a2f999-37f0-49d9-b9fb-5ac823fa4cb3"),
                    ClientName = "MARIA SOUZA",
                    ClientCpf = "222.222.222-22"
                }
            },
            {
                Guid.Parse("d4b2e888-37f0-49d9-b9fb-5ac823fa4cb3"),
                new Client
                {
                    ClientId = Guid.Parse("d4b2e888-37f0-49d9-b9fb-5ac823fa4cb3"),
                    ClientName = "PEDRO GOMES",
                    ClientCpf = "333.333.333-33"
                }
            },
            {
                Guid.Parse("e5c1d777-37f0-49d9-b9fb-5ac823fa4cb3"),
                new Client
                {
                    ClientId = Guid.Parse("e5c1d777-37f0-49d9-b9fb-5ac823fa4cb3"),
                    ClientName = "ANA LIMA",
                    ClientCpf = "444.444.444-44"
                }
            }
        };

        public Task<Client?> GetClientAsync(Guid clientId)
        {
            _clients.TryGetValue(clientId, out var client);
            return Task.FromResult(client);
        }
    }
}
