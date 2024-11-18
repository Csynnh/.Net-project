using System.Security.Claims;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace service;

public interface IShippingMethodService
{
  Task<ShippingMethod> CreateShippingMethod(ShippingMethodRequest shippingMethod);
  Task<List<ShippingMethod>> GetShippingMethods();
  Task DeleteShippingMethod(Guid shippingMethodId);
}
public class ShippingMethodService : IShippingMethodService
{
  private readonly ShippingMethodRepository _repository;

  private readonly IHttpContextAccessor _httpContextAccessor;

  public ShippingMethodService(ShippingMethodRepository repository, IHttpContextAccessor httpContextAccessor)
  {
    _repository = repository;
    _httpContextAccessor = httpContextAccessor;
  }

  public async Task<ShippingMethod> CreateShippingMethod(ShippingMethodRequest shippingMethod)
  {
    try
    {
      // --- Start Check User role
      var user = _httpContextAccessor.HttpContext?.User;
      string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
      if (RoleClaim != "Admin")
      {
        throw new Exception("You do not have permission to list this user info");
      }
      // --- End Check User role

      return await _repository.CreateShippingMethod(shippingMethod);
    }
    catch (Exception ex)
    {
      // Handle the exception as needed, for example, log it and rethrow or return a specific result
      throw new Exception("An error occurred while creating the shipping method", ex);
    }
  }

  public async Task<List<ShippingMethod>> GetShippingMethods()
  {
    try
    {
      // --- Start Check User role
      var user = _httpContextAccessor.HttpContext?.User;
      string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
      if (RoleClaim != "Admin")
      {
        throw new Exception("You do not have permission to list this user info");
      }
      // --- End Check User role

      return await _repository.GetShippingMethods();
    }
    catch (Exception ex)
    {
      // Handle the exception as needed, for example, log it and rethrow or return a specific result
      throw new Exception("An error occurred while getting the shipping methods", ex);
    }
  }

  public async Task DeleteShippingMethod(Guid shippingMethodId)
  {
    try
    {
      // --- Start Check User role
      var user = _httpContextAccessor.HttpContext?.User;
      string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
      if (RoleClaim != "Admin")
      {
        throw new Exception("You do not have permission to list this user info");
      }
      // --- End Check User role

      await _repository.DeleteShippingMethod(shippingMethodId);
    }
    catch (Exception ex)
    {
      // Handle the exception as needed, for example, log it and rethrow or return a specific result
      throw new Exception("An error occurred while deleting the shipping method", ex);
    }
  }
}