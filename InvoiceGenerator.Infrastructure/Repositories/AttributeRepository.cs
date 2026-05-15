using InvoiceGenerator.Contracts.Models;
using InvoiceGenerator.Contracts.Repositories;
using InvoiceGenerator.Infrastructure.Data;
using System.Data;

namespace InvoiceGenerator.Infrastructure.Repositories
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly IDbExecutor _dbExecutor;
        public AttributeRepository(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }
        public async Task UpdateAttribute(AttributeObject attribute)
        {
            const string sql = "[kkur].[ZaktualizujAtrybut]";

            await _dbExecutor.ExecuteAsync(
                sql,
                new
                {
                    ObjectId = attribute.RefNumer,
                    ObjectType = attribute.RefType,
                    ObjectLp = attribute.RefLp,
                    Class = attribute.ClassName,
                    Value = attribute.Value
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
