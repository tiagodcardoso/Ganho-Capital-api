using GanhoDeCapital.Core.Domain.Constants;
using GanhoDeCapital.Core.Domain.Entities;
using GanhoDeCapital.Core.Interfaces;

namespace GanhoDeCapital.Core.Services
{
    public class TaxCalculationService : ITaxCalculationService
    {
        private const decimal TaxRate = 0.20m;
        private const decimal ExemptionThreshold = 20000.00m;

        public decimal CalculateTax(List<Transaction> transactions, out string status)
        {
            if (!ValidateTransactions(transactions, out status))
            {
                return 0;
            }

            var state = new CalculationState();
            decimal totalTax = 0;

            foreach (var transaction in transactions)
            {
                if (transaction.Operation == OperationType.Buy)
                {
                    ProcessBuy(transaction, state);
                }
                else if (transaction.Operation == OperationType.Sell)
                {
                    totalTax += ProcessSell(transaction, state);
                }
            }

            status = OperationStatus.CalculatedSuccessfully;
            return totalTax;
        }

        private bool ValidateTransactions(List<Transaction> transactions, out string status)
        {
            if (transactions == null || transactions.Count == 0)
            {
                status = OperationStatus.CalculationError;
                return false;
            }

            // Verifica se existe pelo menos uma operação de venda
            bool hasSellOperation = transactions.Any(t => t.Operation == OperationType.Sell);

            if (!hasSellOperation)
            {
                status = OperationStatus.CalculationError;
                return false;
            }

            status = string.Empty;
            return true;
        }

        private void ProcessBuy(Transaction transaction, CalculationState state)
        {
            decimal totalCost = state.WeightedAveragePrice * state.TotalShares +
                               transaction.UnitCost * transaction.Quantity;

            state.TotalShares += transaction.Quantity;

            if (state.TotalShares > 0)
            {
                state.WeightedAveragePrice = totalCost / state.TotalShares;
            }
        }

        private decimal ProcessSell(Transaction transaction, CalculationState state)
        {
            decimal saleValue = transaction.UnitCost * transaction.Quantity;
            decimal costBasis = state.WeightedAveragePrice * transaction.Quantity;
            decimal profitOrLoss = saleValue - costBasis;

            state.TotalShares -= transaction.Quantity;

            // Verifica isenção para vendas <= R$ 20.000
            if (saleValue <= ExemptionThreshold)
            {
                // Mesmo isento, o lucro/prejuízo afeta o acumulado
                if (profitOrLoss < 0)
                {
                    state.AccumulatedLoss += Math.Abs(profitOrLoss);
                }
                else
                {
                    // Deduz prejuízo acumulado do lucro
                    if (state.AccumulatedLoss > 0)
                    {
                        if (profitOrLoss >= state.AccumulatedLoss)
                        {
                            profitOrLoss -= state.AccumulatedLoss;
                            state.AccumulatedLoss = 0;
                        }
                        else
                        {
                            state.AccumulatedLoss -= profitOrLoss;
                            profitOrLoss = 0;
                        }
                    }
                }
                return 0; // Isento de imposto
            }

            // Processa prejuízo
            if (profitOrLoss < 0)
            {
                state.AccumulatedLoss += Math.Abs(profitOrLoss);
                return 0;
            }

            // Deduz prejuízo acumulado do lucro
            decimal taxableProfit = profitOrLoss;

            if (state.AccumulatedLoss > 0)
            {
                if (taxableProfit >= state.AccumulatedLoss)
                {
                    taxableProfit -= state.AccumulatedLoss;
                    state.AccumulatedLoss = 0;
                }
                else
                {
                    state.AccumulatedLoss -= taxableProfit;
                    taxableProfit = 0;
                }
            }

            // Calcula imposto sobre lucro tributável
            return taxableProfit * TaxRate;
        }       

        private class CalculationState
        {
            public decimal WeightedAveragePrice { get; set; }
            public int TotalShares { get; set; }
            public decimal AccumulatedLoss { get; set; }
        }
    }
}
