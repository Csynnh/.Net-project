using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace service;

public interface IOderService
{
    Task<IEnumerable<ListOderResponseModel>> ListOderByAccountId(Guid accountId, string status);
    Task<ListOderResponseModel> GetOrderById(Guid orderId);
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
    private readonly NotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;



    public OderService(
        OderRepository oderRepository,
        IHttpContextAccessor httpContextAccessor,
        UserRepository userRepository,
        PaymentMethodRepository paymentMethodRepository,
        ShippingMethodRepository shippingMethodRepository,
        UserStoredInformationRepository userStoredInformationRepository,
        OderDetailRepository invoiceDetailRepository,
        NotificationRepository notificationRepository,
        IHubContext<NotificationHub> hubContext
    )
    {
        _oderRepository = oderRepository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _shippingMethodRepository = shippingMethodRepository;
        _userStoredInformationRepository = userStoredInformationRepository;
        _invoiceDetailRepository = invoiceDetailRepository;
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
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

    public async Task<IEnumerable<ListOderResponseModel>> ListOrderStatusByAccountId(Guid accountId, string status)
    {
        // Fetch orders by status from repository
        IEnumerable<ListOderResponseModel> response = await _oderRepository.ListOrderByStatus(status, accountId);

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
            PaymentMethod paymentMethods = await _paymentMethodRepository.GetPaymentMethodByName(paymentMethod.ToString()!);
            if (paymentMethods == null)
            {
                paymentMethods = await _paymentMethodRepository.AddPaymentMethod(
                    new PaymentMethod
                    {
                        account_id = accountId,
                        payment_method = JsonConvert.SerializeObject(paymentMethod)
                    }
                );
            }
            Guid paymentMethodId = paymentMethods.id;

            ShippingMethod shippingMethods = await _shippingMethodRepository.GetShippingMethodByName(shippingMethod.ToString()!);
            if (shippingMethods == null)
            {
                shippingMethods = await _shippingMethodRepository.CreateShippingMethod(
                    new ShippingMethodRequest
                    {
                        shipping_name = shippingMethod,
                        shipping_cost = 0
                    }
                );
            }
            Guid shippingMethodId = shippingMethods.id;

            dynamic userInfoResponse = _userStoredInformationRepository.GetUserStoredInformationByValues(accountId: accountId, address: userInfo.address, phone: userInfo.phone, name: userInfo.name);
            if (userInfoResponse == null)
            {
                userInfoResponse = _userStoredInformationRepository.CreateUserStoredInformation(
                    accountId,
                    JsonConvert.SerializeObject(userInfo)
                );
            }
            Guid userInfoId = userInfoResponse.id;
            DateTime created_at = DateTime.UtcNow;
            var oder = await _oderRepository.CreateOrder(accountId, total, paymentMethodId, shippingMethodId, userInfoId, created_at);

            foreach (var product in products)
            {

                foreach (var variant in product.variants)
                {
                    Console.WriteLine(variant.id);
                    await _invoiceDetailRepository.CreateOderDetail(new OderDetailRequest
                    {
                        order_id = oder.id,
                        product_variant_id = variant.id,
                        quantity = variant.count,
                        account_id = accountId
                    });
                }
            }
            var result = new
            {
                message = $"Order #{oder.id.ToString().Substring(0, 8)} has been placed and is pending confirmation.",
                createdAt = oder.created_at,
                Id = oder.id
            };
            await NotificationHandler(oder);
            return result;
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism not shown here)
            System.Console.WriteLine($"Error occurred while creating an oder: {ex.Message}");
            throw new Exception($"{ex.Message}");
        }
    }

    private async Task NotificationHandler(OderResponseModel oder)
    {
        var content = new NotificationContent
        {
            message = $"Order #{oder.id.ToString().Substring(0, 8)} has been placed and is pending confirmation.",
            createdAt = oder.created_at,
            Id = oder.id
        };

        Guid notificationId = await _notificationRepository.InsertNotification(new Notification
        {
            content = JsonConvert.SerializeObject(content),
            created_at = oder.created_at,
            type = "ODER_NOTIFICATION"
        });

        await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", new NotificationResponseModel
        {
            id = notificationId,
            content = content,
            created_at = oder.created_at,
            is_read = false
        });
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

    public async Task<ListOderResponseModel> GetOrderById(Guid orderId)
    {
        var response = await _oderRepository.GetOrderById(orderId);
        return response;
    }
}
