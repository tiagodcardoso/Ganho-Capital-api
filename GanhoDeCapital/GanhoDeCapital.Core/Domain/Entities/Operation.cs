using GanhoDeCapital.Core.Domain.Entities;

namespace GanhoDeCapital.Core.Domain.Entites
{
    public class Operation
    {
        public long OperationId { get; set; }
        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientCpf { get; set; }
        public decimal? Tax { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<Transaction> Transactions { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }
}
