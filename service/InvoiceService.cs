using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
using infrastructure.QueryModels;
using infrastructure.Repositories;

namespace service;

public class InvoiceService
{
    private readonly InvoiceRepository _invoiceRepository;

    public InvoiceService(InvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public IEnumerable<Invoice> GetInvoiceForFeed()
    {
        return _invoiceRepository.GetInvoiceForFeed();
    }

    public Invoice CreateInvoice(Guid accountId, decimal total, Status status, Checkout_method checkoutMethod, Shipping_method shippingMethod)
    {
        return _invoiceRepository.CreateInvoice(accountId, total, status, checkoutMethod, shippingMethod);
    }

    public Invoice UpdateInvoice(Guid invoiceId, decimal total, Status status, Checkout_method checkoutMethod, Shipping_method shippingMethod)
    {
        return _invoiceRepository.UpdateInvoice(invoiceId, total, status, checkoutMethod, shippingMethod);
    }

    public void DeleteInvoice(Guid id )
    {
        var result = _invoiceRepository.DeleteInvoice(id );
        if (!result)
        {
            throw new Exception("Could not delete invoice");
        }
    }
}
