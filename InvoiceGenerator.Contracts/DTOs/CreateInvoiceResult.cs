namespace InvoiceGenerator.Contracts.DTOs
{
    public class CreateInvoiceResult
    {
        public int InvoiceId { get; set; }
        public int InvoiceType { get; set; }
        public int InvoiceCompany { get; set; }
        public int InvoiceLp { get; set; }
    }
}
