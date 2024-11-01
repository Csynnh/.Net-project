using System.Security.Claims;
using System.Text.Json;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace service;

public interface IPaymentMethodService
{
  Task<paymentMethodResponse> AddPaymentMethod(PaymentMethodRequest paymentMethod);
  Task<List<paymentMethodResponse>> GetPaymentMethodByAccountId(Guid accountId);
  Task<string> DeletePaymentMethod(Guid paymentMethodId);
}
public class PaymentMethodService : IPaymentMethodService
{
  private readonly PaymentMethodRepository _repository;

  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly UserRepository _userRepository;

  public PaymentMethodService(PaymentMethodRepository repository, IHttpContextAccessor httpContextAccessor, UserRepository userRepository)
  {
    _repository = repository;
    _httpContextAccessor = httpContextAccessor;
    _userRepository = userRepository;
  }

  public async Task<paymentMethodResponse> AddPaymentMethod(PaymentMethodRequest paymentMethod)
  {
    try
    {
      // --- Start Check User role
      var user = _httpContextAccessor.HttpContext?.User;
      string UsernameClaim = user?.FindFirst(ClaimTypes.Name)?.Value!;
      string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
      User? AccountRequest = await _userRepository.GetUserByAccountIdAsync(paymentMethod.account_id);
      if (AccountRequest == null)
      {
        throw new Exception("Account not found");
      }
      else if (AccountRequest != null && AccountRequest.Username != UsernameClaim && RoleClaim != "Admin")
      {
        throw new Exception("You do not have permission to list this user info");
      }
      // --- End Check User role

      var paymentMethodData = new PaymentMethod
      {
        account_id = paymentMethod.account_id,
        payment_method = JsonSerializer.Serialize(paymentMethod.payment_method.ToUpper())
      };
      PaymentMethod paymentRessult = await _repository.AddPaymentMethod(paymentMethodData);
      paymentMethodResponse paymentMethodResponse = new paymentMethodResponse
      {
        id = paymentRessult.id,
        account_id = paymentRessult.account_id,
        payment_method = JsonSerializer.Deserialize<object>(paymentRessult.payment_method)!
      };
      return paymentMethodResponse;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  public async Task<List<paymentMethodResponse>> GetPaymentMethodByAccountId(Guid accountId)
  {
    try
    {
      // --- Start Check User role
      var user = _httpContextAccessor.HttpContext?.User;
      string UsernameClaim = user?.FindFirst(ClaimTypes.Name)?.Value!;
      string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
      User? AccountRequest = await _userRepository.GetUserByAccountIdAsync(accountId);
      if (AccountRequest == null)
      {
        throw new Exception("Account not found");
      }
      else if (AccountRequest != null && AccountRequest.Username != UsernameClaim && RoleClaim != "Admin")
      {
        throw new Exception("You do not have permission to list this user info");
      }
      // --- End Check User role

      List<PaymentMethod> paymentRessult = await _repository.GetPaymentMethodByAccountId(accountId);
      List<paymentMethodResponse> paymentMethodResponses = paymentRessult.Select(paymentMethod => new paymentMethodResponse
      {
        id = paymentMethod.id,
        account_id = paymentMethod.account_id,
        payment_method = JsonSerializer.Deserialize<object>(paymentMethod.payment_method)!
      }).ToList();
      return paymentMethodResponses;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  public async Task<string> DeletePaymentMethod(Guid paymentMethodId)
  {
    try
    {
      // --- Start Check User role
      var user = _httpContextAccessor.HttpContext?.User;
      string UsernameClaim = user?.FindFirst(ClaimTypes.Name)?.Value!;
      string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
      PaymentMethod? PaymentMethodRequest = await _repository.GetPaymentMethodById(paymentMethodId);
      if (PaymentMethodRequest == null)
      {
        throw new Exception("Payment method not found");
      }
      User? AccountRequest = await _userRepository.GetUserByAccountIdAsync(PaymentMethodRequest.account_id);
      if (AccountRequest == null)
      {
        throw new Exception("Account not found");
      }
      else if (AccountRequest != null && AccountRequest.Username != UsernameClaim && RoleClaim != "Admin")
      {
        throw new Exception("You do not have permission to list this user info");
      }
      // --- End Check User role

      await _repository.DeletePaymentMethod(paymentMethodId);
      string paymentMethod = JsonSerializer.Deserialize<PaymentMethodModel>(PaymentMethodRequest.payment_method)!.payment_name;
      return $"Payment method {paymentMethod} deleted successfully";
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }
}