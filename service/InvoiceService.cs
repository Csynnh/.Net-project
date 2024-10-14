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

    public IEnumerable<Invoice> GetInvoices()
    {
        return _invoiceRepository.GetAllInvoices();
    }

    public Invoice CreateInvoice(Guid account_id, DateTime created_date, decimal total, Status status, Checkout_method checkout_method, Shipping_method shipped_method)
    {
        return _invoiceRepository.CreateInvoice(account_id, created_date, total, status, checkout_method, shipped_method);
    }

    public Invoice UpdateInvoice(Guid id , Guid account_id, DateTime created_date, decimal total, Status status, Checkout_method checkout_method, Shipping_method shipped_method)
    {
        return _invoiceRepository.UpdateInvoice(id , account_id, created_date, total, status, checkout_method, shipped_method);
    }

    public void DeleteInvoice(int id )
    {
        var result = _invoiceRepository.DeleteInvoice(id );
        if (!result)
        {
            throw new Exception("Could not delete invoice");
        }
    }
}
