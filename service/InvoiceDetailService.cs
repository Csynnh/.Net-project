using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using infrastructure.Repositories;

namespace service;

public class InvoiceDetailService
{
    private readonly InvoiceDetailRepository _invoiceDetailRepository;

    public InvoiceDetailService(InvoiceDetailRepository invoiceDetailRepository)
    {
        _invoiceDetailRepository = invoiceDetailRepository;
    }

    public IEnumerable<InvoiceDetail> GetInvoiceDetailForFeed()
    {
        return _invoiceDetailRepository.GetInvoiceDetailForFeed();
    }

    public InvoiceDetail CreateInvoiceDetail(Guid invoiceId, Guid productId, int amount, decimal price)
    {
        return _invoiceDetailRepository.CreateInvoiceDetail(invoiceId , productId, amount, price);
    }

    public InvoiceDetail UpdateInvoiceDetail(Guid invoiceId, Guid productId, int amount, decimal price)
    {
        return _invoiceDetailRepository.UpdateInvoiceDetail(invoiceId , productId, amount, price);
    }

    public void DeleteInvoiceDetail(Guid invoiceId, Guid productId)
    {
        var result = _invoiceDetailRepository.DeleteInvoiceDetail(invoiceId , productId);
        if (!result)
        {
            throw new Exception("Could not delete invoice detail");
        }
    }
}
