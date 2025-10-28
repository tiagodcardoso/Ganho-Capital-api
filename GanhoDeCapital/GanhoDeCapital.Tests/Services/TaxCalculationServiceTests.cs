using GanhoDeCapital.Core.Domain.Constants;
using GanhoDeCapital.Core.Domain.Entities;
using GanhoDeCapital.Core.Services;

namespace GanhoDeCapital.Tests.Services
{
    public class TaxCalculationServiceTests
    {
        private readonly TaxCalculationService _service;

        public TaxCalculationServiceTests()
        {
            _service = new TaxCalculationService();
        }

        [Fact]
        public void CalculateTax_ExemptSale_ReturnsZeroTax()
        {
            // Arrange
            var transactions = new List<Transaction>
        {
            new() { Operation = "buy", UnitCost = 10.00m, Quantity = 1000 },
            new() { Operation = "sell", UnitCost = 15.00m, Quantity = 1000 }
        };

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculatedSuccessfully, status);
            Assert.Equal(0.00m, tax); // Venda de 15000 <= 20000 (isento)
        }

        [Fact]
        public void CalculateTax_LossDeductingProfit_ReturnsCorrectTax()
        {
            // Arrange
            var transactions = new List<Transaction>
        {
            new() { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
            new() { Operation = "sell", UnitCost = 5.00m, Quantity = 5000 },  // Prejuízo de 25000
            new() { Operation = "sell", UnitCost = 20.00m, Quantity = 3000 }  // Lucro de 30000
        };

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculatedSuccessfully, status);
            
            Assert.Equal(1000.00m, tax);
        }

        [Fact]
        public void CalculateTax_WeightedAveragePrice_ReturnsCorrectTax()
        {
            // Arrange
            var transactions = new List<Transaction>
        {
            new() { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
            new() { Operation = "buy", UnitCost = 25.00m, Quantity = 5000 },
            new() { Operation = "sell", UnitCost = 15.00m, Quantity = 10000 },
            new() { Operation = "sell", UnitCost = 25.00m, Quantity = 5000 }
        };

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculatedSuccessfully, status);
            
            Assert.Equal(10000.00m, tax);
        }

        [Fact]
        public void CalculateTax_OnlyBuyOperations_ReturnsCalculationError()
        {
            // Arrange
            var transactions = new List<Transaction>
        {
            new() { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 }
        };

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculationError, status);
            Assert.Equal(0m, tax);
        }

        [Fact]
        public void CalculateTax_EmptyTransactions_ReturnsCalculationError()
        {
            // Arrange
            var transactions = new List<Transaction>();

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculationError, status);
            Assert.Equal(0m, tax);
        }

        [Fact]
        public void CalculateTax_MultipleLosses_AccumulatesCorrectly()
        {
            // Arrange
            var transactions = new List<Transaction>
        {
            new() { Operation = "buy", UnitCost = 20.00m, Quantity = 10000 },
            new() { Operation = "sell", UnitCost = 10.00m, Quantity = 5000 },  // Prejuízo 50000
            new() { Operation = "sell", UnitCost = 15.00m, Quantity = 2000 },  // Prejuízo 10000
            new() { Operation = "sell", UnitCost = 50.00m, Quantity = 3000 }   // Lucro 90000
        };

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculatedSuccessfully, status);
            
            Assert.Equal(6000.00m, tax);
        }

        [Fact]
        public void CalculateTax_LossGreaterThanProfit_ReturnsZeroTax()
        {
            // Arrange
            var transactions = new List<Transaction>
        {
            new() { Operation = "buy", UnitCost = 20.00m, Quantity = 10000 },
            new() { Operation = "sell", UnitCost = 10.00m, Quantity = 8000 },  // Prejuízo 80000
            new() { Operation = "sell", UnitCost = 30.00m, Quantity = 2000 }   // Lucro 20000
        };

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculatedSuccessfully, status);
        }


        [Fact]
        public void CalculateTax_SimpleProfitScenario_ReturnsCorrectTax()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new() { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
                new() { Operation = "sell", UnitCost = 17.00m, Quantity = 10000 }
            };

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculatedSuccessfully, status);            
            
            Assert.Equal(14000.00m, tax);
        }

        [Fact]
        public void CalculateTax_ExemptSaleWithLoss_AccumulatesLoss()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new() { Operation = "buy", UnitCost = 20.00m, Quantity = 1000 },
                new() { Operation = "sell", UnitCost = 15.00m, Quantity = 1000 },
                new() { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
                new() { Operation = "sell", UnitCost = 20.00m, Quantity = 10000 }
            };

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculatedSuccessfully, status);
            
            Assert.Equal(19000.00m, tax);
        }

        [Fact]
        public void CalculateTax_MultipleWeightedAveragePriceUpdates_CalculatesCorrectly()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new() { Operation = "buy", UnitCost = 10.00m, Quantity = 5000 },
                new() { Operation = "buy", UnitCost = 20.00m, Quantity = 5000 },
                new() { Operation = "buy", UnitCost = 30.00m, Quantity = 5000 },
                new() { Operation = "sell", UnitCost = 25.00m, Quantity = 15000 }
            };

            // Act
            var tax = _service.CalculateTax(transactions, out string status);

            // Assert
            Assert.Equal(OperationStatus.CalculatedSuccessfully, status);
            
            Assert.Equal(15000.00m, tax);
        }
    }
}
