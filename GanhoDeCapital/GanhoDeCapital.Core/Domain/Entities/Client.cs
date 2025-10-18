namespace GanhoDeCapital.Core.Domain.Entites
{
    public class Client
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientCpf { get; set; } = string.Empty;
    }
}
