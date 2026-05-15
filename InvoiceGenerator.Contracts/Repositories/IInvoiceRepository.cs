namespace InvoiceGenerator.Contracts.Repositories
{
    public interface IInvoiceRepository
    {
        public Task<string> GetInvoiceName(int id, int type);
    }
}
