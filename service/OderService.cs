using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
using infrastructure.QueryModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace service;

public interface IOderService
{
    Task<IEnumerable<ListOderResponseModel>> ListOderByAccountId(Guid accountId);
    Task<OderResponseModel> CreateNewOder(Guid accountId, decimal total, Guid paymentMethodId, Guid shippingMethodId, Guid storedInformationId, string status);

}

public class OderService : IOderService
{
    private readonly OderRepository _oderRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserRepository _userRepository;

    public OderService(OderRepository oderRepository, IHttpContextAccessor httpContextAccessor, UserRepository userRepository)
    {
        _oderRepository = oderRepository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
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

    public async Task<OderResponseModel> CreateNewOder(Guid accountId, decimal total, Guid paymentMethodId, Guid shippingMethodId, Guid storedInformationId, string status)
    {
        try
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

            return await _oderRepository.CreateOrder(accountId, total, paymentMethodId, shippingMethodId, storedInformationId, status);
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
