using InvoiceGenerator.Contracts.DTOs;
using InvoiceGenerator.Contracts.Models;

namespace InvoiceGenerator.Contracts.Services
{
    public interface IXlApiService
    {
        public void Login();
        public void Logout();
        public CreateInvoiceResult CreateInvoice(List<Order> orders);
    }
}
