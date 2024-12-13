using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Text.Json;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace service;

public class UserStoredInformationService
{
    private readonly UserStoredInformationRepository _UserStoredInformationRepository;
    private readonly UserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserStoredInformationService(UserStoredInformationRepository UserStoredInformationRepository, UserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _UserStoredInformationRepository = UserStoredInformationRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IEnumerable<UserInformationResponse>> ListUserStoredInformation(Guid accountId)
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

            IEnumerable<UserStoredInformation> res = _UserStoredInformationRepository.ListUserStoredInformation(accountId);
            List<UserInformationResponse> userStoredInformationResponses = new List<UserInformationResponse>();
            foreach (UserStoredInformation userInfo in res)
            {
                UserInformationModel userStoredInfoModel = JsonSerializer.Deserialize<UserInformationModel>(userInfo.info)!;
                userStoredInformationResponses.Add(new UserInformationResponse()
                {
                    id = userInfo.id,
                    account_id = userInfo.account_id,
                    info = userStoredInfoModel
                });
            }
            return userStoredInformationResponses;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("ListUserInfo::An error occurred while listing user info ", ex);
        }
    }

    public async Task<UserInformationModel?> CreateUserStoredInformation(Guid accountId, UserInformationRequest info)
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
                throw new Exception("You do not have permission to delete this user info");
            }

            int NextSequence = _UserStoredInformationRepository.GetLastSequence() + 1;
            UserInformationModel userinfoModel = new UserInformationModel()
            {
                id = NextSequence,
                account_id = accountId,
                address = info.address,
                name = info.name,
                phone = info.phone
            };
            string UserInfo = JsonSerializer.Serialize(info);
            UserStoredInformation res = _UserStoredInformationRepository.CreateUserStoredInformation(accountId, UserInfo);
            if (res.info != null)
            {
                UserInformationModel resUserInfo = JsonSerializer.Deserialize<UserInformationModel>(res.info)!;
                return new UserInformationModel()
                {
                    id = resUserInfo.id,
                    address = resUserInfo.address,
                    name = resUserInfo.name,
                    phone = resUserInfo.phone,
                    account_id = res.account_id
                };
            }
            return null;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("CreateUserInfo::An error occurred while creating user info", ex);
        }
    }

    public async Task<string?> UpdateUserStoredInformation(Guid userStoredInfoId, Guid accountId, UserInformationRequest info)
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
                throw new Exception("You do not have permission to delete this user info");
            }

            UserStoredInformation? existingUserInfo = _UserStoredInformationRepository.GetUserStoredInformationById(userStoredInfoId) ?? throw new Exception("User info not found");
            UserInformationModel existingUserInfoModel = JsonSerializer.Deserialize<UserInformationModel>(existingUserInfo.info)!;
            UserInformationModel userInfoModel = new()
            {
                address = info.address,
                name = info.name,
                phone = info.phone,
                id = existingUserInfoModel.id,
                account_id = existingUserInfo.account_id
            };

            string updatedInfoJson = JsonSerializer.Serialize(info);
            UserStoredInformation updatedUserInfo = _UserStoredInformationRepository.UpdateUserStoredInformation(userStoredInfoId, updatedInfoJson);

            if (updatedUserInfo.info != null)
            {
                UserInformationModel resUserInfo = JsonSerializer.Deserialize<UserInformationModel>(updatedUserInfo.info)!;
                return $"UserInfo updated successfully: {resUserInfo.address}";
            }
            return null;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception($"UpdateUserInfo::An error occurred while updating user info: {ex}");
        }
    }

    public async Task<string> DeleteUserStoredInformation(Guid id, Guid accountId)
    {
        try
        {
            // Check user role
            var AccountRequest = await _userRepository.GetUserByAccountIdAsync(accountId);
            var user = _httpContextAccessor.HttpContext?.User;
            string UsernameClaim = user?.FindFirst(ClaimTypes.Name)?.Value!;
            if (AccountRequest != null && AccountRequest.Username != UsernameClaim && AccountRequest.Role != "Admin")
            {
            throw new Exception("You do not have permission to delete this user info");
            }
            // user info exists before deleting
            var UserInfoRequest = _UserStoredInformationRepository.GetUserStoredInformationById(id) ?? throw new Exception("User info not found");
            var result = _UserStoredInformationRepository.DeleteUserStoredInformation(id);
            if (!result)
            {
            throw new Exception("Could not delete UserInfo");
            }
            return $"UserInfo deleted successfully: {UserInfoRequest.info}";
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception($"DeleteUserInfo::An error occurred while deleting user info: {ex}");
        }
    }
}
