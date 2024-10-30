using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Text.Json;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace service;

public class UserAddressService
{
    private readonly UserAddressRepository _UserAddressRepository;
    private readonly UserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAddressService(UserAddressRepository UserAddressRepository, UserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _UserAddressRepository = UserAddressRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IEnumerable<UserAddressResponse>> ListUserAddress(Guid accountId)
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
                throw new Exception("You do not have permission to list this user address");
            }

            IEnumerable<UserAddress> res = _UserAddressRepository.ListUserAddress(accountId);
            List<UserAddressResponse> userAddressResponses = new List<UserAddressResponse>();
            foreach (UserAddress userAddress in res)
            {
                UserAddressModel userAddressModel = JsonSerializer.Deserialize<UserAddressModel>(userAddress.address)!;
                userAddressResponses.Add(new UserAddressResponse()
                {
                    id = userAddress.id,
                    account_id = userAddress.account_id,
                    address = userAddressModel
                });
            }
            return userAddressResponses;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("ListUserAddress::An error occurred while listing user addresses ", ex);
        }
    }

    public async Task<UserAddressModel?> CreateUserAddress(Guid accountId, UserAddressRequest address)
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
                throw new Exception("You do not have permission to delete this user address");
            }

            int NextSequence = _UserAddressRepository.GetLastSequence() + 1;
            UserAddressModel userAddressModel = new UserAddressModel()
            {
                id = NextSequence,
                account_id = accountId,
                address = address.address,
                name = address.name,
                phone = address.phone
            };
            string UserAddresss = JsonSerializer.Serialize(userAddressModel);
            UserAddress res = _UserAddressRepository.CreateUserAddress(accountId, UserAddresss);
            if (res.address != null)
            {
                UserAddressModel resUserAddress = JsonSerializer.Deserialize<UserAddressModel>(res.address)!;
                return new UserAddressModel()
                {
                    id = resUserAddress.id,
                    address = resUserAddress.address,
                    name = resUserAddress.name,
                    phone = resUserAddress.phone,
                    account_id = res.account_id
                };
            }
            return null;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("CreateUserAddress::An error occurred while creating user address", ex);
        }
    }

    public async Task<string?> UpdateUserAddress(Guid userAddressId, Guid accountId, UserAddressRequest address)
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
                throw new Exception("You do not have permission to delete this user address");
            }

            UserAddress? existingUserAddress = _UserAddressRepository.GetUserAddressById(userAddressId) ?? throw new Exception("User address not found");
            UserAddressModel existingUserAddressModel = JsonSerializer.Deserialize<UserAddressModel>(existingUserAddress.address)!;
            UserAddressModel userAddressModel = new()
            {
                address = address.address,
                name = address.name,
                phone = address.phone,
                id = existingUserAddressModel.id,
                account_id = existingUserAddress.account_id
            };

            string updatedAddressJson = JsonSerializer.Serialize(address);
            UserAddress updatedUserAddress = _UserAddressRepository.UpdateUserAddress(userAddressId, updatedAddressJson);

            if (updatedUserAddress.address != null)
            {
                UserAddressModel resUserAddress = JsonSerializer.Deserialize<UserAddressModel>(updatedUserAddress.address)!;
                return $"UserAddress updated successfully: {resUserAddress.address}";
            }
            return null;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception($"UpdateUserAddress::An error occurred while updating user address: {ex}");
        }
    }

    public async Task<string> DeleteUserAddress(Guid id, Guid accountId)
    {
        try
        {
            // Check user role
            var AccountRequest = await _userRepository.GetUserByAccountIdAsync(accountId);
            var user = _httpContextAccessor.HttpContext?.User;
            string UsernameClaim = user?.FindFirst(ClaimTypes.Name)?.Value!;
            if (AccountRequest != null && AccountRequest.Username != UsernameClaim && AccountRequest.Role != "Admin")
            {
            throw new Exception("You do not have permission to delete this user address");
            }
            // user address exists before deleting
            var UserAddressRequest = _UserAddressRepository.GetUserAddressById(id) ?? throw new Exception("User address not found");
            var result = _UserAddressRepository.DeleteUserAddress(id);
            if (!result)
            {
            throw new Exception("Could not delete UserAddress");
            }
            return $"UserAddress deleted successfully: {UserAddressRequest.address}";
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception($"DeleteUserAddress::An error occurred while deleting user address: {ex}");
        }
    }
}
