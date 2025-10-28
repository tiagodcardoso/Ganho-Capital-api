using GanhoDeCapital.Core.Services;
using GanhoDeCapital.Core.Domain.Constants;
using GanhoDeCapital.Core.Domain.Entities;
using GanhoDeCapital.Core.Interfaces;
using GanhoDeCapital.Core.Domain.DTOs;
using GanhoDeCapital.Core.Domain.Entites;
using Moq;

namespace GanhoDeCapital.Tests.Services
{
    public class ProcessTaxServiceTests
    {
        private readonly Mock<ITaxCalculationService> _taxCalculationServiceMock;
        private readonly Mock<IClientService> _clientServiceMock;
        private readonly Mock<IOperationRepository> _repositoryMock;
        private readonly ProcessTaxService _service;

        public ProcessTaxServiceTests()
        {
            _taxCalculationServiceMock = new Mock<ITaxCalculationService>();
            _clientServiceMock = new Mock<IClientService>();
            _repositoryMock = new Mock<IOperationRepository>();

            _service = new ProcessTaxService(
                _taxCalculationServiceMock.Object,
                _clientServiceMock.Object,
                _repositoryMock.Object);
        }

        [Fact]
        public async Task ProcessOperations_NullClientId_ReturnsEnrichmentError()
        {
            // Arrange
            var requests = new List<OperationRequest>
        {
            new()
            {
                OperationId = 1,
                ClientId = null,
                Operations = new List<TransactionDto>
                {
                    new() { Operation = "buy", UnitCost = 10m, Quantity = 100 }
                }
            }
        };

            _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => op);

            // Act
            var results = await _service.ProcessOperationsAsync(requests);

            // Assert
            Assert.Single(results);
            Assert.Equal(OperationStatus.EnrichmentError, results[0].Status);
            Assert.Null(results[0].Tax);
            Assert.Null(results[0].ClientName);
            Assert.Null(results[0].ClientCpf);
        }

        [Fact]
        public async Task ProcessOperations_ClientNotFound_ReturnsEnrichmentError()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var requests = new List<OperationRequest>
        {
            new()
            {
                OperationId = 1,
                ClientId = clientId,
                Operations = new List<TransactionDto>
                {
                    new() { Operation = "buy", UnitCost = 10m, Quantity = 100 }
                }
            }
        };

            _clientServiceMock.Setup(c => c.GetClientAsync(clientId))
                .ReturnsAsync((Client?)null);

            _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => op);

            // Act
            var results = await _service.ProcessOperationsAsync(requests);

            // Assert
            Assert.Single(results);
            Assert.Equal(OperationStatus.EnrichmentError, results[0].Status);
            Assert.Null(results[0].Tax);
        }

        [Fact]
        public async Task ProcessOperations_ValidRequest_ReturnsSuccessfulCalculation()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client
            {
                ClientId = clientId,
                ClientName = "JOAO SILVA",
                ClientCpf = "111.111.111-11"
            };
            var requests = new List<OperationRequest>
        {
            new()
            {
                OperationId = 1,
                ClientId = clientId,
                Operations = new List<TransactionDto>
                {
                    new() { Operation = "buy", UnitCost = 10m, Quantity = 10000 },
                    new() { Operation = "sell", UnitCost = 20m, Quantity = 10000 }
                }
            }
        };

            _clientServiceMock.Setup(c => c.GetClientAsync(clientId))
                .ReturnsAsync(client);
            
            _taxCalculationServiceMock.Setup(t => t.CalculateTax(
                It.IsAny<List<Transaction>>(), out It.Ref<string>.IsAny))
                .Callback((List<Transaction> trans, out string status) =>
                {
                    status = OperationStatus.CalculatedSuccessfully;
                })
                .Returns(20000m);

            _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => op);

            // Act
            var results = await _service.ProcessOperationsAsync(requests);

            // Assert
            Assert.Single(results);
            Assert.Equal(OperationStatus.CalculatedSuccessfully, results[0].Status);
            Assert.Equal(20000m, results[0].Tax);
            Assert.Equal("JOAO SILVA", results[0].ClientName);
            Assert.Equal("111.111.111-11", results[0].ClientCpf);
        }

        [Fact]
        public async Task ProcessOperations_CalculationError_ReturnsErrorStatus()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client { ClientId = clientId, ClientName = "MARIA SOUZA", ClientCpf = "222.222.222-22" };
            var requests = new List<OperationRequest>
        {
            new()
            {
                OperationId = 1,
                ClientId = clientId,
                Operations = new List<TransactionDto> { new() { Operation = "buy", UnitCost = 10m, Quantity = 10000 } }
            }
        };

            _clientServiceMock.Setup(c => c.GetClientAsync(clientId))
                .ReturnsAsync(client);
            
            _taxCalculationServiceMock.Setup(t => t.CalculateTax(
                It.IsAny<List<Transaction>>(), out It.Ref<string>.IsAny))
                .Callback((List<Transaction> trans, out string status) =>
                {
                    status = OperationStatus.CalculationError;
                })
                .Returns(0m);

            _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => op);

            // Act
            var results = await _service.ProcessOperationsAsync(requests);

            // Assert
            Assert.Single(results);
            Assert.Equal(OperationStatus.CalculationError, results[0].Status);
            Assert.Null(results[0].Tax);
        }

        [Fact]
        public async Task ProcessOperations_MultipleRequests_ProcessesAll()
        {
            // Arrange
            var clientId1 = Guid.NewGuid();
            var clientId2 = Guid.NewGuid();

            var client1 = new Client { ClientId = clientId1, ClientName = "CLIENT 1", ClientCpf = "111.111.111-11" };
            var client2 = new Client { ClientId = clientId2, ClientName = "CLIENT 2", ClientCpf = "222.222.222-22" };

            var requests = new List<OperationRequest>
        {
            new()
            {
                OperationId = 1,
                ClientId = clientId1,
                Operations = new List<TransactionDto>
                {
                    new() { Operation = "buy", UnitCost = 10m, Quantity = 10000 },
                    new() { Operation = "sell", UnitCost = 20m, Quantity = 10000 }
                }
            },
            new()
            {
                OperationId = 2,
                ClientId = clientId2,
                Operations = new List<TransactionDto>
                {
                    new() { Operation = "buy", UnitCost = 15m, Quantity = 5000 },
                    new() { Operation = "sell", UnitCost = 25m, Quantity = 5000 }
                }
            }
        };

            _clientServiceMock.Setup(c => c.GetClientAsync(clientId1)).ReturnsAsync(client1);
            _clientServiceMock.Setup(c => c.GetClientAsync(clientId2)).ReturnsAsync(client2);

            _taxCalculationServiceMock.Setup(t => t.CalculateTax(
                It.IsAny<List<Transaction>>(), out It.Ref<string>.IsAny))
                .Callback((List<Transaction> trans, out string status) =>
                {
                    status = OperationStatus.CalculatedSuccessfully;
                })
                .Returns(10000m);

            _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => op);

            // Act
            var results = await _service.ProcessOperationsAsync(requests);

            // Assert
            Assert.Equal(2, results.Count);
            Assert.All(results, r => Assert.Equal(OperationStatus.CalculatedSuccessfully, r.Status));
        }
    }
}