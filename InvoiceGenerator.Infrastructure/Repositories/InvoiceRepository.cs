using InvoiceGenerator.Contracts.Repositories;
using InvoiceGenerator.Infrastructure.Data;
using System.Data;

namespace InvoiceGenerator.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IDbExecutor _dbExecutor;
        public InvoiceRepository(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<string> GetInvoiceName(int id, int type)
        {
            return await _dbExecutor.QuerySingleOrDefaultAsync<string>(
                "[dbo].[GetDocumentName]",
                new
                {
                    id,
                    type
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
