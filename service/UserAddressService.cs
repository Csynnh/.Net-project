using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using infrastructure.Repositories;

namespace service;

public class UserAddressService
{
    private readonly UserAddressRepository _UserAddressRepository;

    public UserAddressService(UserAddressRepository UserAddressRepository)
    {
        _UserAddressRepository = UserAddressRepository;
    }

    public IEnumerable<UserAddress> GetUserAddressForFeed()
    {
        return _UserAddressRepository.GetUserAddressForFeed();
    }

    public UserAddress CreateUserAddress(Guid accountId, string address)
    {
        return _UserAddressRepository.CreateUserAddress(accountId, address);
    }

    public UserAddress UpdateUserAddress(Guid userAddressId, string address)
    {
        return _UserAddressRepository.UpdateUserAddress(userAddressId, address);
    }

    public void DeleteUserAddress(Guid id)
    {
        var result = _UserAddressRepository.DeleteUserAddress(id);
        if (!result)
        {
            throw new Exception("Could not delete UserAddress");
        }
    }
}
