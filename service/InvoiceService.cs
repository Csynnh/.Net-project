using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
using infrastructure.QueryModels;
using infrastructure.Repositories;

namespace service;

public class OderService
{
    private readonly OderRepository _oderRepository;

    public OderService(OderRepository oderRepository)
    {
        _oderRepository = oderRepository;
    }

    public IEnumerable<OderResponseModel> ListOderByAccountId(Guid accountId)
    {
        return _oderRepository.ListOrderByAccountId(accountId);
    }

    // public Invoice CreateInvoice(Guid accountId, decimal total, Status status, Checkout_method checkoutMethod, Shipping_method shippingMethod)
    // {
    //     return _oderRepository.CreateInvoice(accountId, total, status, checkoutMethod, shippingMethod);
    // }

    // public Invoice UpdateInvoice(Guid invoiceId, decimal total, Status status, Checkout_method checkoutMethod, Shipping_method shippingMethod)
    // {
    //     return _oderRepository.UpdateInvoice(invoiceId, total, status, checkoutMethod, shippingMethod);
    // }

    // public void DeleteInvoice(Guid id )
    // {
    //     var result = _oderRepository.DeleteInvoice(id );
    //     if (!result)
    //     {
    //         throw new Exception("Could not delete invoice");
    //     }
    // }
}
