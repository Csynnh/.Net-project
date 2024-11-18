using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace service;

public interface IOderDetailService
{
    Task<OderDetailResponse> CreateOderDetail(OderDetailRequest oderDetail);
}
public class OderDetailService : IOderDetailService
{
    private readonly OderDetailRepository _invoiceDetailRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserRepository _userRepository;
    private readonly OderRepository _oderRepository;

    public OderDetailService(OderDetailRepository invoiceDetailRepository, IHttpContextAccessor httpContextAccessor, UserRepository userRepository, OderRepository oderRepository)
    {
        _invoiceDetailRepository = invoiceDetailRepository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _oderRepository = oderRepository;
    }

    public async Task<OderDetailResponse> CreateOderDetail(OderDetailRequest oderDetail)
    {

        try
        {
            // Check user role
            var user = _httpContextAccessor.HttpContext?.User;
            string UsernameClaim = user?.FindFirst(ClaimTypes.Name)?.Value!;
            string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
            Console.WriteLine(oderDetail.account_id);
            User? AccountRequest = await _userRepository.GetUserByAccountIdAsync(oderDetail.account_id);

            if (AccountRequest == null)
            {
                throw new Exception("Account not found");
            }
            else if (AccountRequest != null && AccountRequest.Username != UsernameClaim && RoleClaim != "Admin")
            {
                throw new Exception("You do not have permission to list this user info");
            }
            // End check user role
            Console.WriteLine($"UsernameClaim: {UsernameClaim} - {AccountRequest?.Username}");

            // Check if oder exist
            ListOderResponseModel? oder = await _oderRepository.GetOrderById(oderDetail.order_id) ?? throw new Exception("Oder not found");
            // End check if oder exist

            // TODO Check if product variant exist
            return await _invoiceDetailRepository.CreateOderDetail(oderDetail);
        }
        catch (System.Exception ex)
        {

            throw new Exception($"Failed to create oder detail {ex.Message}");
        }
    }
}
