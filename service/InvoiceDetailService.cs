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

    public InvoiceDetail CreateInvoiceDetail(int idHoaDon, int idHangHoa, int soLuong, decimal gia)
    {
        return _invoiceDetailRepository.CreateInvoiceDetail(idHoaDon, idHangHoa, soLuong, gia);
    }

    public InvoiceDetail UpdateInvoiceDetail(int idHoaDon, int idHangHoa, int soLuong, decimal gia)
    {
        return _invoiceDetailRepository.UpdateInvoiceDetail(idHoaDon, idHangHoa, soLuong, gia);
    }

    public void DeleteInvoiceDetail(int idHoaDon, int idHangHoa)
    {
        var result = _invoiceDetailRepository.DeleteInvoiceDetail(idHoaDon, idHangHoa);
        if (!result)
        {
            throw new Exception("Could not delete invoice detail");
        }
    }
}
