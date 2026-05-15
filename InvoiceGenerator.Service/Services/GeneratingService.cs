using InvoiceGenerator.Contracts.Models;
using InvoiceGenerator.Contracts.Repositories;
using InvoiceGenerator.Contracts.Services;

namespace InvoiceGenerator.Service.Services
{
    public class GeneratingService
    {
        private readonly ILogger<GeneratingService> _logger;
        private readonly IOrderRespository _orderRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IAttributeRepository _attributeRepo;
        private readonly IXlApiService _xlApiService;
        public GeneratingService(ILogger<GeneratingService> logger, IOrderRespository orderRepo, IAttributeRepository attributeRepo, IXlApiService xlApiService, IInvoiceRepository invoiceRepo)
        {
            _logger = logger;
            _orderRepo = orderRepo;
            _attributeRepo = attributeRepo;
            _xlApiService = xlApiService;
            _invoiceRepo = invoiceRepo;
        }

        public async Task GenerateInvoices()
        {
            try
            {
                var orders = await _orderRepo.GetOrders();
                _logger.LogInformation("Retrieved {OrderCount} orders for invoice generation.", orders.Count());

                foreach (var order in orders)
                {
                    try
                    {
                        var result = _xlApiService.CreateInvoice(order);
                        string invoiceName = await _invoiceRepo.GetInvoiceName(result.InvoiceId, result.InvoiceType);
                        _logger.LogInformation("Successfully generated invoice {InvoiceName} ({InvoiceId}) for order {OrderName} ({OrderId})", invoiceName, result.InvoiceId, order.Name, order.Id);

                        try
                        {
                            var attribute = new AttributeObject
                            {
                                RefCompany = result.InvoiceCompany,
                                RefLp = result.InvoiceLp,
                                RefNumer = result.InvoiceId,
                                RefType = result.InvoiceType,
                                ClassName = "StatusWMS",
                                Value = "Do realizacji"
                            };

                            await _attributeRepo.UpdateAttribute(attribute);
                            _logger.LogInformation("Updated StatusWMS attribute for invoice {InvoiceName} ({InvoiceId}) on 'do realizacji'.", invoiceName, result.InvoiceId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to update StatusWMS attribute for invoice {InvoiceName} ({InvoiceId}).", invoiceName, result.InvoiceId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating invoice for order {OrderName} ({OrderId})", order.Name, order.Id);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoices");
                throw;
            }
        }
    }
}
