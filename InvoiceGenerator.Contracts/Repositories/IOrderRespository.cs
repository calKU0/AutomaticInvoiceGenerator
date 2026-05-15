using InvoiceGenerator.Contracts.Models;

namespace InvoiceGenerator.Contracts.Repositories
{
    public interface IOrderRespository
    {
        public Task<IEnumerable<Order>> GetOrders();
    }
}
