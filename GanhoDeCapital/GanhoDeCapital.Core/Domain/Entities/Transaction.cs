namespace GanhoDeCapital.Core.Domain.Entities
{
    public class Transaction
    {
        public string Operation { get; set; } = string.Empty;
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
    }

}
