using GanhoDeCapital.Core.Domain.Entities;

namespace GanhoDeCapital.Core.Interfaces
{
    public interface ITaxCalculationService
    {
        decimal CalculateTax(List<Transaction> transactions, out string status);
    }
}
