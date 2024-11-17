using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace service;

public interface IOderService
{
    Task<IEnumerable<ListOderResponseModel>> ListOderByAccountId(Guid accountId);
    Task<string> CreateNewOder(
        Guid accountId,
        decimal total,
        string paymentMethod,
        string shippingMethod,
        UserInformationRequest userInfo,
        List<ProductCheckout> products
    );

}

public class OderService : IOderService
{
    private readonly OderRepository _oderRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserRepository _userRepository;
    private readonly PaymentMethodRepository _paymentMethodRepository;
    private readonly ShippingMethodRepository _shippingMethodRepository;
    private readonly UserStoredInformationRepository _userStoredInformationRepository;
    private readonly OderDetailRepository _invoiceDetailRepository;


    public OderService(
        OderRepository oderRepository,
        IHttpContextAccessor httpContextAccessor,
        UserRepository userRepository,
        PaymentMethodRepository paymentMethodRepository,
        ShippingMethodRepository shippingMethodRepository,
        UserStoredInformationRepository userStoredInformationRepository,
        OderDetailRepository invoiceDetailRepository
    ) {
        _oderRepository = oderRepository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _shippingMethodRepository = shippingMethodRepository;
        _userStoredInformationRepository = userStoredInformationRepository;
        _invoiceDetailRepository = invoiceDetailRepository;
    }

    public async Task<IEnumerable<ListOderResponseModel>> ListOderByAccountId(Guid accountId)
    {
        // Check user role
        var user = _httpContextAccessor.HttpContext?.User;
        string UsernameClaim = user?.FindFirst(ClaimTypes.Name)?.Value!;
        string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
        User? AccountRequest = await _userRepository.GetUserByAccountIdAsync(accountId);
        if (AccountRequest != null && AccountRequest.Username != UsernameClaim && RoleClaim != "Admin")
        {
            throw new Exception("You do not have permission to list this user info");
        }
        // End check user role

        IEnumerable<ListOderResponseModel> response = await _oderRepository.ListOrderByAccountId(accountId);

        return response;
    }

    public async Task<string> CreateNewOder(Guid accountId,
        decimal total,
        string paymentMethod,
        string shippingMethod,
        UserInformationRequest userInfo,
        List<ProductCheckout> products
    )
    {
        try
        {
            Authorization authorization = new Authorization(_httpContextAccessor, _userRepository);
            await authorization.IsValidUser(accountId);

            PaymentMethod paymentMethods = await _paymentMethodRepository.GetPaymentMethodByName(paymentMethod.ToString()!);
            Guid paymentMethodId = paymentMethods.id;

            ShippingMethod shippingMethods = await _shippingMethodRepository.GetShippingMethodByName(shippingMethod.ToString()!);
            Guid shippingMethodId = shippingMethods.id;

            Guid userInfoId = _userStoredInformationRepository.GetUserStoredInformationByValues(accountId: accountId, address: userInfo.address, phone: userInfo.phone, name: userInfo.name).id;

            var oder = await _oderRepository.CreateOrder(accountId, total, paymentMethodId, shippingMethodId, userInfoId);

            foreach (var product in products)
            {

                foreach (var variant in product.variants)
                {
                    await _invoiceDetailRepository.CreateOderDetail(new OderDetailRequest
                    {
                        order_id = oder.id,
                        product_variant_id = variant.id,
                        quantity = variant.count,
                        account_id = accountId
                    });
                }
            }
            return $"Order created successfully with id: {oder.id}";
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism not shown here)
            throw new Exception("An error occurred while creating the order", ex);
        }
    }

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
