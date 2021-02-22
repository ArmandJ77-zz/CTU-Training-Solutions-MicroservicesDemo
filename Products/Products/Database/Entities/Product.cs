namespace Products.Database.Entities
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; } = 0;
    }
}
