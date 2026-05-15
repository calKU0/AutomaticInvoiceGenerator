using InvoiceGenerator.Contracts.Models;

namespace InvoiceGenerator.Contracts.Repositories
{
    public interface IAttributeRepository
    {
        public Task UpdateAttribute(AttributeObject attribute);
    }
}
