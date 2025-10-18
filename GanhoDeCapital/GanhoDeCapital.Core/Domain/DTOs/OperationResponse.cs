using System.Text.Json.Serialization;

namespace GanhoDeCapital.Core.Domain.DTOs
{
    public class OperationResponse
    {
        [JsonPropertyName("operation-id")]
        public long OperationId { get; set; }

        [JsonPropertyName("client-id")]
        public Guid? ClientId { get; set; }

        [JsonPropertyName("client-name")]
        public string? ClientName { get; set; }

        [JsonPropertyName("client-cpf")]
        public string? ClientCpf { get; set; }

        [JsonPropertyName("tax")]
        public decimal? Tax { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
