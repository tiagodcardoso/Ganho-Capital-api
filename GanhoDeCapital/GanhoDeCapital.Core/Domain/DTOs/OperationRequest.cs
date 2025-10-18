using System.Text.Json.Serialization;

namespace GanhoDeCapital.Core.Domain.DTOs
{
    public class OperationRequest
    {
        [JsonPropertyName("operation-id")]
        public long OperationId { get; set; }

        [JsonPropertyName("client-id")]
        public Guid? ClientId { get; set; }

        [JsonPropertyName("operations")]
        public List<TransactionDto> Operations { get; set; } = new();
    }

    public class TransactionDto
    {
        [JsonPropertyName("operation")]
        public string Operation { get; set; } = string.Empty;

        [JsonPropertyName("unit-cost")]
        public decimal UnitCost { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
