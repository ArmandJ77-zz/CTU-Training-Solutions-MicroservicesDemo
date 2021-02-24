namespace Orders.Database.Entities
{
    public class Order
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int Qty { get; set; } = 0;
    }
}
