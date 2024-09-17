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

    public IEnumerable<InvoiceDetail> GetInvoiceDetails()
    {
        return _invoiceDetailRepository.GetInvoiceDetails();
    }

    public InvoiceDetail CreateInvoiceDetail(int invoices_id , int product_id, int amount, decimal price)
    {
        return _invoiceDetailRepository.CreateInvoiceDetail(invoices_id , product_id, amount, price);
    }

    public InvoiceDetail UpdateInvoiceDetail(int invoices_id , int product_id, int amount, decimal price)
    {
        return _invoiceDetailRepository.UpdateInvoiceDetail(invoices_id , product_id, amount, price);
    }

    public void DeleteInvoiceDetail(int invoices_id , int product_id)
    {
        var result = _invoiceDetailRepository.DeleteInvoiceDetail(invoices_id , product_id);
        if (!result)
        {
            throw new Exception("Could not delete invoice detail");
        }
    }
}
