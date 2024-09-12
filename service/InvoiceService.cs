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

    public Invoice CreateInvoice(int idTaiKhoan, DateTime ngayXuatHoaDon, decimal tongTien, string trangThai)
    {
        return _invoiceRepository.CreateInvoice(idTaiKhoan, ngayXuatHoaDon, tongTien, trangThai);
    }

    public Invoice UpdateInvoice(int idHoaDon, int idTaiKhoan, DateTime ngayXuatHoaDon, decimal tongTien, string trangThai)
    {
        return _invoiceRepository.UpdateInvoice(idHoaDon, idTaiKhoan, ngayXuatHoaDon, tongTien, trangThai);
    }

    public void DeleteInvoice(int idHoaDon)
    {
        var result = _invoiceRepository.DeleteInvoice(idHoaDon);
        if (!result)
        {
            throw new Exception("Could not delete invoice");
        }
    }
}
