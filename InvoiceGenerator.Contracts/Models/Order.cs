namespace InvoiceGenerator.Contracts.Models
{
    public class Order
    {
        public string Name { get; set; } = default!;
        public int Id { get; set; }
        public int Company { get; set; }
        public int Type { get; set; }
        public int Lp { get; set; }
        public string DestinationCountry { get; set; } = default!;
        public string Courier { get; set; } = default!;
        public int PaymentType { get; set; }
        public int PaymentDueDateClarion { get; set; }
        public int PriceGroup { get; set; }
        public string? Description { get; set; }
        public string ClientAcronym { get; set; } = default!;
        public int DefaultDocumentType { get; set; }
        public string? PackingRequirements { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
