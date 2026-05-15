namespace InvoiceGenerator.Contracts.Models
{
    public class OrderItem
    {
        public string Code { get; set; }
        public int Lp { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public string Currency { get; set; }
    }
}
