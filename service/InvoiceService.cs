using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
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
        return _invoiceRepository.GetInvoices();
    }

    public Invoice CreateInvoice(int account_id, DateTime created_date, decimal price, string status)
    {
        return _invoiceRepository.CreateInvoice(account_id, created_date, price, status);
    }

    public Invoice UpdateInvoice(int id , int account_id, DateTime created_date, decimal price, string status)
    {
        return _invoiceRepository.UpdateInvoice(id , account_id, created_date, price, status);
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
