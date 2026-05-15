using InvoiceGenerator.Contracts.Models;
using InvoiceGenerator.Contracts.Repositories;
using InvoiceGenerator.Infrastructure.Data;
using System.Data;

namespace InvoiceGenerator.Infrastructure.Repositories
{
    public class OrderRespository : IOrderRespository
    {
        private readonly IDbExecutor _dbExecutor;
        public OrderRespository(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }
        public async Task<IEnumerable<Order>> GetOrders()
        {
            var orderDictionary = new Dictionary<int, Order>();

            await _dbExecutor.QueryAsync<Order, OrderItem, Order>(
                "[dbo].[GetOrdersToAutomaticFV]",
                (order, item) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var existingOrder))
                    {
                        existingOrder = order;
                        existingOrder.Items = new List<OrderItem>();
                        orderDictionary.Add(order.Id, existingOrder);
                    }

                    if (item != null)
                        existingOrder.Items.Add(item);

                    return existingOrder;
                },
                splitOn: "Code",
                commandType: CommandType.StoredProcedure
            );

            return orderDictionary.Values;
        }
    }
}
