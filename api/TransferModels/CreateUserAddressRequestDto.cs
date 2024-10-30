
namespace infrastructure.DataModels;

public class CreateUserAddressRequestDto
{
    public Guid account_id { get; set; }
    public UserAddressRequest address  { get; set; }
}