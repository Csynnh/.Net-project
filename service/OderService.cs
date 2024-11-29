using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace service;

public interface IOderService
{
    Task<IEnumerable<ListOderResponseModel>> ListOderByAccountId(Guid accountId, string status);
    Task<dynamic> CreateNewOder(
        Guid accountId,
        decimal total,
        string paymentMethod,
        string shippingMethod,
        UserInformationRequest userInfo,
        List<ProductCheckout> products
    );
    Task<IEnumerable<RetrieveChartDataResponse>> RetrieveChartData(string ChartType);
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

    public async Task<IEnumerable<ListOderResponseModel>> ListOderByAccountId(Guid accountId, string status)
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

        IEnumerable<ListOderResponseModel> response = await _oderRepository.ListOrderByAccountId(accountId, status);

        return response;
    }

 public async Task<IEnumerable<ListOderResponseModel>> ListOrderByStatus(string status)
{
    // Check user role
    var user = _httpContextAccessor.HttpContext?.User;
    string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
    
    // Only allow Admin to list orders by status
    if (RoleClaim != "Admin")
    {
        throw new Exception("You do not have permission to list orders by status");
    }
   // End check user role

    // Fetch orders by status from repository
    IEnumerable<ListOderResponseModel> response = await _oderRepository.ListOrderByStatus(status);

    return response;
}
public async Task<List<object>> GetTotalOrders() // Trả vè số lượng order cho mỗi status
{
     // Check user role
    var user = _httpContextAccessor.HttpContext?.User;
    string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
    // Only allow Admin to list orders by status
    if (RoleClaim != "Admin")
    {
        throw new Exception("You do not have permission to list orders by status");
    }
    // Lấy danh sách các trạng thái và tổng số lượng từ Repository
    var summaries = await _oderRepository.GetTotalOrdersGroupedByStatus();

    // Tính tổng tất cả các trạng thái
    int totalAll = summaries.Sum(s => s.Total);

    // Thêm một đối tượng "ALL" vào danh sách
    var result = new List<object>
    {
        new { Status = "ALL", Total = totalAll }
    };

    // Gộp đối tượng "ALL" với danh sách trạng thái khác
    result.AddRange(summaries.Select(s => new { s.Status, s.Total }));

    return result;
}

public async Task<bool> UpdateToNextOrderStatus(Guid orderId)
{
     // Check user role
    // var user = _httpContextAccessor.HttpContext?.User;
    // string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
    // // Only allow Admin to list orders by status
    // if (RoleClaim != "Admin")
    // {
    //     throw new Exception("You do not have permission to list orders by status");
    // }

    // Lấy thông tin đơn hàng
    var order = await _oderRepository.GetOrderById(orderId);
    if (order == null)
        throw new Exception("Order not found");

    // Cập nhật trạng thái tiếp theo
    return await _oderRepository.UpdateToNextOrderStatus(orderId, order.status);
}

    public async Task<dynamic> CreateNewOder(Guid accountId,
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
            await authorization.ValidateUser(accountId);

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
            return new {
                message = $"Order #{oder.id.ToString().Substring(0, 8)} has been placed and is pending confirmation.",
                createdAt = oder.created_at,
                Id = oder.id
            };
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism not shown here)
            throw new Exception("An error occurred while creating the order", ex);
        }
    }

    public async Task<IEnumerable<RetrieveChartDataResponse>> RetrieveChartData(string ChartType)
    {
        DateTime StartDateTime;
        DateTime EndDateTime;
        if (ChartType == "DAILY")
        {
            DateTime today = DateTime.Today;
            StartDateTime = today.Date;
            EndDateTime = today.Date.AddHours(23).AddMinutes(59);
        }
        else if (ChartType == "WEEKLY")
        {
            StartDateTime = DateTime.Today.AddDays(-7);
            EndDateTime = StartDateTime.AddDays(6).AddHours(23).AddMinutes(59);
        }
        else if (ChartType == "MONTHLY")
        {
            StartDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDateTime = StartDateTime.AddMonths(1).AddDays(-1);
        }
        else
        {
            throw new Exception("Invalid chart type");
        }

        var response = await _oderRepository.RetrieveChartData(StartDateTime, EndDateTime);
        return response;
    }
}
